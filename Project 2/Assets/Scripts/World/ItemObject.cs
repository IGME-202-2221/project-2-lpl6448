using UnityEngine;

/// <summary>
/// Represents the visual object for an ItemType, which can be carried or stacked.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class ItemObject : MonoBehaviour
{
    /// <summary>
    /// Transform where the ItemObject's position is shifted if the ItemObject is being carried
    /// </summary>
    public Transform carryingPivot;

    /// <summary>
    /// Transform where the ItemObject's position is shifted if the ItemObject is being stacked
    /// </summary>
    public Transform stackedPivot;

    /// <summary>
    /// Vertical height of this ItemObject, used to stack other ItemObjects on top of this one
    /// </summary>
    public float stackedHeight;

    /// <summary>
    /// Shifts this ItemObject's local position to the carryingPivot
    /// </summary>
    public void PositionObjectForCarrying()
    {
        transform.localPosition -= carryingPivot.localPosition;
    }

    /// <summary>
    /// Shifts this ItemObject's local position to the stackedPivot
    /// and moves it up by the baseHeight
    /// </summary>
    public void StackObject(float baseHeight)
    {
        transform.localPosition += Vector3.up * baseHeight - stackedPivot.localPosition;
    }
}