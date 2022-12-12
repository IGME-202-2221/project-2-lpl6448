using UnityEngine;

/// <summary>
/// Utility component that creates a unique Material for a Renderer and can set its color
/// without affecting the color of other Renderers.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class MaterialColor : MonoBehaviour
{
    /// <summary>
    /// Reference to the Renderer that will have a unique color
    /// </summary>
    public Renderer rendererToChange;

    /// <summary>
    /// To initialize, creates a unique Material for the rendererToChange
    /// </summary>
    private void Start()
    {
        rendererToChange.sharedMaterial = rendererToChange.material;
    }

    /// <summary>
    /// Updates the color property of the rendererToChange's unique material
    /// </summary>
    /// <param name="color">Color that the rendererToChange will appear</param>
    public void SetColor(Color color)
    {
        rendererToChange.sharedMaterial.color = color;
    }
}