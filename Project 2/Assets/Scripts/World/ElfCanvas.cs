using UnityEngine;
using UnityEngine.UI;

public class ElfCanvas : MonoBehaviour
{
    public Canvas canvas;

    private void LateUpdate()
    {
        canvas.transform.forward = Camera.main.transform.forward;
    }
}