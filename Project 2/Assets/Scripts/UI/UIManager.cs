using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public CraftingPanel craftingPanel;

    public GamePanel gamePanel;

    public void RefreshCanBuild()
    {
        craftingPanel.RefreshCanBuild();
        gamePanel.UpdateCircleIndicatorColors();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}