using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller script (unfinished) that takes player input to rotate the camera around the map.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class CameraController : MonoBehaviour
{
    public Camera cameraComponent;

    public float lookSensitivity;

    public float lookLerp;

    public float minPitch;

    public float maxPitch;

    public Transform orbitReference;

    public float horizontalOrbitOffset;

    public float orbitMoveSpeed;

    public Vector3 orbitPositionExtents;

    public float orbitMoveLerp;

    public float zoomSensitivity;

    public float zoomLerp;

    public float zoomSpring;

    public float minZoomDistance;

    public float maxZoomDistance;

    private bool draggingCamera = false;

    private Vector3 currentCameraAngles;

    private Vector3 goalCameraAngles;

    private float currentOrbitDistanceLog;

    private float goalOrbitDistanceLog;

    private float orbitDistanceVelocity;

    private Vector3 currentOrbitPosition;

    private Vector3 goalOrbitPosition;

    private Vector3 currentMoveDir;

    public void DragDelta(InputAction.CallbackContext context)
    {
        if (draggingCamera)
        {
            Vector2 mouseMovePixels = context.ReadValue<Vector2>();
            Vector2 mouseMoveUv = mouseMovePixels / Screen.height; // Make sensitivity independent of resolution

            goalCameraAngles += new Vector3(-mouseMoveUv.y, mouseMoveUv.x) * lookSensitivity;
            goalCameraAngles.x = Mathf.Clamp(goalCameraAngles.x, minPitch, maxPitch);
        }
    }

    public void CameraDrag(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            draggingCamera = true;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (context.canceled)
        {
            draggingCamera = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (draggingCamera)
        {
            goalOrbitDistanceLog -= context.ReadValue<float>() * zoomSensitivity;
            goalOrbitDistanceLog = Mathf.Clamp(goalOrbitDistanceLog, Mathf.Log(minZoomDistance), Mathf.Log(maxZoomDistance));
        }
    }

    public void MoveOrbit(InputAction.CallbackContext context)
    {
        Vector2 movement2D = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movement2D.x, 0, movement2D.y);
        currentMoveDir = movement.normalized;
    }

    private void Start()
    {
        currentCameraAngles = transform.eulerAngles;
        goalCameraAngles = currentCameraAngles;

        currentOrbitDistanceLog = Mathf.Log(Vector3.Distance(orbitReference.transform.position, transform.position));
        goalOrbitDistanceLog = currentOrbitDistanceLog;

        currentOrbitPosition = orbitReference.position;
        goalOrbitPosition = currentOrbitPosition;
    }

    private void Update()
    {
        // Update camera rotation
        float lookLerpT = 1 - Mathf.Pow(lookLerp, Time.deltaTime);
        currentCameraAngles = new Vector3(
            Mathf.LerpAngle(currentCameraAngles.x, goalCameraAngles.x, lookLerpT),
            Mathf.LerpAngle(currentCameraAngles.y, goalCameraAngles.y, lookLerpT),
            Mathf.LerpAngle(currentCameraAngles.z, goalCameraAngles.z, lookLerpT));
        transform.eulerAngles = currentCameraAngles;

        // Update orbit position
        if (draggingCamera)
        {
            Vector3 transformedMoveDir = transform.right * currentMoveDir.x + Vector3.Cross(transform.right, Vector3.up) * currentMoveDir.z;
            goalOrbitPosition += transformedMoveDir * orbitMoveSpeed * Time.deltaTime;
            goalOrbitPosition.x = Mathf.Clamp(goalOrbitPosition.x, -orbitPositionExtents.x, orbitPositionExtents.x);
            goalOrbitPosition.z = Mathf.Clamp(goalOrbitPosition.z, -orbitPositionExtents.z, orbitPositionExtents.z);
        }
        float orbitMoveLerpT = 1 - Mathf.Pow(orbitMoveLerp, Time.deltaTime);
        currentOrbitPosition = Vector3.Lerp(currentOrbitPosition, goalOrbitPosition, orbitMoveLerpT);
        orbitReference.position = currentOrbitPosition;

        // Update zoom and orbit
        orbitDistanceVelocity += (goalOrbitDistanceLog - currentOrbitDistanceLog) * zoomSpring * Time.deltaTime;
        orbitDistanceVelocity *= 1 - Mathf.Clamp01(2 * Mathf.Sqrt(zoomSpring) * Time.deltaTime);
        currentOrbitDistanceLog += orbitDistanceVelocity * Time.deltaTime;
        float orbitDistance = Mathf.Exp(currentOrbitDistanceLog);
        PositionCamera(orbitReference.position, orbitDistance);
    }

    private void PositionCamera(Vector3 orbitPos, float orbitDistance)
    {
        Plane forwardPlane = new Plane(transform.forward, orbitPos);
        transform.position = orbitReference.position - transform.forward * orbitDistance;
        Ray shiftRay = cameraComponent.ViewportPointToRay(new Vector3(-horizontalOrbitOffset / cameraComponent.aspect / 2 + 0.5f, 0.5f));
        if (forwardPlane.Raycast(shiftRay, out float t))
        {
            transform.position = shiftRay.GetPoint(t) - transform.forward * orbitDistance;
        }
    }
}