using UnityEngine;

public class MaterialColor : MonoBehaviour
{
    public Renderer rendererToChange;

    private void Start()
    {
        rendererToChange.sharedMaterial = rendererToChange.material;
    }

    public void SetColor(Color color)
    {
        rendererToChange.sharedMaterial.color = color;
    }
}