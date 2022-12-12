using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller script that takes player input to rotate, move, and zoom the camera.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Reference to the Camera component for this CameraController, used to center the
    /// Camera's position in the GamePanel
    /// </summary>
    public Camera cameraComponent;

    /// <summary>
    /// Sensitivity factor for rotating the Camera
    /// </summary>
    public float lookSensitivity;

    /// <summary>
    /// Lerp factor used to smooth out mouse movements/Camera rotations
    /// </summary>
    public float lookLerp;

    /// <summary>
    /// Minimum pitch (Euler x-value) of the Camera in degrees
    /// </summary>
    public float minPitch;

    /// <summary>
    /// Maximum pitch (Euler x-value) of the Camera in degrees
    /// </summary>
    public float maxPitch;

    /// <summary>
    /// Positional reference that the Camera always points toward
    /// </summary>
    public Transform orbitReference;

    /// <summary>
    /// Vertical viewport units, transformed later to horizontal units so that the Camera
    /// is positioned, regardless of aspect ratio, in the middle of the GamePanel
    /// </summary>
    public float horizontalOrbitOffset;

    /// <summary>
    /// Speed at which the orbitTransform moves around the map with user input.
    /// </summary>
    public float orbitMoveSpeed;

    /// <summary>
    /// Extents of a bounding box that the orbitTransform always stays inside of
    /// </summary>
    public Vector3 orbitPositionExtents;

    /// <summary>
    /// Lerp factor used to smooth the motion of the orbitTransform
    /// </summary>
    public float orbitMoveLerp;

    /// <summary>
    /// Sensitivity factor for zooming the Camera in and out using user input
    /// </summary>
    public float zoomSensitivity;

    /// <summary>
    /// Spring coefficient for smoothing the motion of the zoom amount
    /// </summary>
    public float zoomSpring;

    /// <summary>
    /// Minimum distance from the orbitTransform to the Camera
    /// </summary>
    public float minZoomDistance;

    /// <summary>
    /// Maximum distance from the orbitTransform to the Camera
    /// </summary>
    public float maxZoomDistance;

    /// <summary>
    /// Whether the Camera is currently being dragged (rotated, moved, or zoomed) with user input
    /// </summary>
    private bool draggingCamera = false;

    /// <summary>
    /// Euler angles of the Camera's current rotation
    /// </summary>
    private Vector3 currentCameraAngles;

    /// <summary>
    /// Euler angles of the Camera's goal rotation
    /// </summary>
    private Vector3 goalCameraAngles;

    /// <summary>
    /// Natural log (used for interpolation) of the Camera's current orbit distance
    /// </summary>
    private float currentOrbitDistanceLog;

    /// <summary>
    /// Natural log (used for interpolation) of the Camera's goal orbit distance
    /// </summary>
    private float goalOrbitDistanceLog;

    /// <summary>
    /// Current velocity (in logarithmic space) of the Camera's orbit distance
    /// </summary>
    private float orbitDistanceVelocity;

    /// <summary>
    /// Current world-space position of the orbitTransform
    /// </summary>
    private Vector3 currentOrbitPosition;

    /// <summary>
    /// Goal world-space position of the orbitTransform
    /// </summary>
    private Vector3 goalOrbitPosition;

    /// <summary>
    /// This frame's input movement direction, used to move the orbitTransform if necessary
    /// </summary>
    private Vector3 currentMoveDir;

    /// <summary>
    /// Called by the InputManager to rotate the Camera with user input
    /// </summary>
    /// <param name="context">CallbackContext containing information about this user interaction</param>
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

    /// <summary>
    /// Called by the InputManager to enable or disable draggingCamera and to lock/unlock the cursor
    /// </summary>
    /// <param name="context">CallbackContext containing information about this user interaction</param>
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

    /// <summary>
    /// Called by the InputManager to increase or decrease the Camera's goal orbit distance
    /// </summary>
    /// <param name="context">CallbackContext containing information about this user interaction</param>
    public void Zoom(InputAction.CallbackContext context)
    {
        if (draggingCamera)
        {
            goalOrbitDistanceLog -= context.ReadValue<float>() * zoomSensitivity;
            goalOrbitDistanceLog = Mathf.Clamp(goalOrbitDistanceLog, Mathf.Log(minZoomDistance), Mathf.Log(maxZoomDistance));
        }
    }

    /// <summary>
    /// Called by the InputManager to update the moveDir, which is used to move the orbitTransform
    /// </summary>
    /// <param name="context">CallbackContext containing information about this user interaction</param>
    public void MoveOrbit(InputAction.CallbackContext context)
    {
        Vector2 movement2D = context.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movement2D.x, 0, movement2D.y);
        currentMoveDir = movement.normalized;
    }

    /// <summary>
    /// Initializes all of the current and goal Camera properties
    /// </summary>
    private void Start()
    {
        currentCameraAngles = transform.eulerAngles;
        goalCameraAngles = currentCameraAngles;

        currentOrbitDistanceLog = Mathf.Log(Vector3.Distance(orbitReference.transform.position, transform.position));
        goalOrbitDistanceLog = currentOrbitDistanceLog;

        currentOrbitPosition = orbitReference.position;
        goalOrbitPosition = currentOrbitPosition;
    }

    /// <summary>
    /// Every frame, performs interpolation on all of the current and goal Camera properties
    /// and updates the Camera with them
    /// </summary>
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

    /// <summary>
    /// Positions the Camera such that it is in the center of the GamePanel by shifting the orbit position
    /// </summary>
    /// <param name="orbitPos">Current world-space orbitTransform position</param>
    /// <param name="orbitDistance">Current distance from the shifted orbit position to the Camera</param>
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