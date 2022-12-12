using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains UI information (if the Elf has a task or not) about a particular Elf.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class ElfCanvas : MonoBehaviour
{
    /// <summary>
    /// Reference to the camera-facing Canvas
    /// </summary>
    public Canvas canvas;

    /// <summary>
    /// Every frame, updates the Canvas so that it always faces the Camera
    /// </summary>
    private void LateUpdate()
    {
        canvas.transform.forward = Camera.main.transform.forward;
    }
}