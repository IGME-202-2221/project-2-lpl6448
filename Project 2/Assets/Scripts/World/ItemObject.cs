using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public Transform carryingPivot;

    public Transform stackedPivot;

    public float stackedHeight;

    public void PositionObjectForCarrying()
    {
        transform.localPosition -= carryingPivot.localPosition;
    }

    public void StackObject(float baseHeight)
    {
        transform.localPosition += Vector3.up * baseHeight - stackedPivot.localPosition;
    }
}