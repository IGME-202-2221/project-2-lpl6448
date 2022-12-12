using UnityEngine;

/// <summary>
/// Manager class that contains the major UI panels (CraftingPanel and GamePanel)
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton UIManager in the scene
    /// </summary>
    public static UIManager Instance;

    /// <summary>
    /// Reference to the game's CraftingPanel
    /// </summary>
    public CraftingPanel craftingPanel;

    /// <summary>
    /// Reference to the game's GamePanel
    /// </summary>
    public GamePanel gamePanel;

    /// <summary>
    /// Updates all of the CraftingPanel's CraftingItemUIs and all of the Stations'
    /// circle indicators depending on the current active modes and items
    /// </summary>
    public void RefreshCanBuild()
    {
        craftingPanel.RefreshCanBuild();
        gamePanel.UpdateCircleIndicatorColors();
    }

    /// <summary>
    /// Initializes the singleton by setting it equal to this UIManager instance if it
    /// has not already been set
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}