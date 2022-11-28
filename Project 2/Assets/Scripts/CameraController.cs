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
    public float lookSensitivity;

    public float minPitch;

    public float maxPitch;

    public float zoomSensitivity;

    public float minZoomDistance;

    public float maxZoomDistance;

    private Vector3 cameraRotation;

    private void Start()
    {
        cameraRotation = transform.eulerAngles;
    }

    private void Update()
    {
        cameraRotation += new Vector3();
    }
}