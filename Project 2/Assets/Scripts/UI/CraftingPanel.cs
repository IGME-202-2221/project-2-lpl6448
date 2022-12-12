using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents the Crafting Panel, which contains all of the items that must be crafted.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class CraftingPanel : MonoBehaviour
{
    /// <summary>
    /// Container/parent for all CraftingItemUIs
    /// </summary>
    public Transform itemsContainer;

    /// <summary>
    /// CraftingItemUI prefab to instantiate for a craftable ItemType
    /// </summary>
    public CraftingItemUI craftableItemPrefab;

    /// <summary>
    /// CraftingItemUI prefab to instantiate for an uncraftable (resource) ItemType
    /// </summary>
    public CraftingItemUI resourceItemPrefab;

    /// <summary>
    /// List of CraftingItemUIs at the root level (for requested items)
    /// </summary>
    private List<CraftingItemUI> rootItemsUI = new List<CraftingItemUI>();

    /// <summary>
    /// Whether the panel is currently in Craft Mode or not
    /// </summary>
    private bool craftMode = false;

    /// <summary>
    /// Reference to the CraftingItemUI that is currently being considered when in Craft Mode
    /// </summary>
    private CraftingItemUI craftItemUI = null;

    /// <summary>
    /// Creates CraftingItemUIs for all root ItemTypes in the provided list and for ingredient/resource ItemTypes
    /// </summary>
    /// <param name="rootItems">List of requested ItemTypes at the root level of the hierarchy</param>
    public void Populate(List<ItemType> rootItems)
    {
        foreach (ItemType rootItem in rootItems)
        {
            rootItemsUI.Add(CreateItemUI(rootItem, 0, 1));
        }
        RefreshCanBuild();
    }

    /// <summary>
    /// Recursively updates whether each of the CraftingItemUIs in the hierarchy can be built
    /// </summary>
    public void RefreshCanBuild()
    {
        foreach (CraftingItemUI itemUI in rootItemsUI)
        {
            RefreshCanBuild(itemUI);
        }
    }

    /// <summary>
    /// Recursively updates whether each of the CraftingItemUIs in the hierarchy can be built
    /// <param name="itemUI"/>CraftingItemUI that will be updated</param>
    /// </summary>
    private void RefreshCanBuild(CraftingItemUI itemUI)
    {
        itemUI.SetCanCraft(itemUI.item.craftable && Taskmaster.Instance.CanBuildItem(itemUI.item));

        foreach (CraftingItemUI nestedItem in itemUI.nestedItems)
        {
            RefreshCanBuild(nestedItem);
        }
    }

    /// <summary>
    /// Enters Craft Mode considering the given CraftingItemUI, entering Selection Mode
    /// in the GamePanel
    /// </summary>
    /// <param name="itemUI">CraftingItemUI to be crafted</param>
    public void EnterCraftMode(CraftingItemUI itemUI)
    {
        if (craftMode && craftItemUI == itemUI)
        {
            ExitCraftMode();
            UIManager.Instance.gamePanel.DisableSelectionMode();
        }
        else
        {
            craftItemUI = itemUI;
            craftMode = true;

            UIManager.Instance.gamePanel.EnableSelectionMode(itemUI.item);
        }
    }

    /// <summary>
    /// Exits Craft Mode, resetting the CraftingPanel's state accordingly
    /// </summary>
    public void ExitCraftMode()
    {
        craftMode = false;
        craftItemUI = null;
    }

    /// <summary>
    /// Instantiates and initializes a CraftingItemUI for the given item and recursively
    /// creates CraftingItemUIs for all nested ingredient/resource items
    /// </summary>
    /// <param name="item">ItemType to create a CraftingItemUI for</param>
    /// <param name="indent">Number of indents/levels for the new CraftingItemUI</param>
    /// <param name="multiplier">Number of the given item required for crafting</param>
    /// <returns>The instantiated CraftingItemUI for the given item</returns>
    private CraftingItemUI CreateItemUI(ItemType item, int indent, int multiplier)
    {
        CraftingItemUI itemUI = Instantiate(GetPrefab(item), itemsContainer, false);
        itemUI.InitializeItem(item, indent, multiplier);
        if (item.craftable)
        {
            // Get counts of each ItemType required
            Dictionary<ItemType, int> ingredientCounts = new Dictionary<ItemType, int>();
            foreach (ItemType ingredient in item.ingredients)
            {
                if (ingredientCounts.ContainsKey(ingredient))
                {
                    ingredientCounts[ingredient] += 1;
                }
                else
                {
                    ingredientCounts[ingredient] = 1;
                }
            }

            bool containsCraftableIngredient = false;
            foreach (KeyValuePair<ItemType, int> ingredientCount in ingredientCounts)
            {
                itemUI.nestedItems.Add(CreateItemUI(ingredientCount.Key, indent + 1, ingredientCount.Value));
                if (ingredientCount.Key.craftable)
                {
                    containsCraftableIngredient = true;
                }
            }

            if (containsCraftableIngredient)
            {
                itemUI.Expand();
            }
            else
            {
                itemUI.Collapse();
            }
        }
        return itemUI;
    }

    /// <summary>
    /// Gets the CraftingItemUI prefab to use (craftableItemPrefab or resourceItemPrefab)
    /// based on the given item and whether it is craftable
    /// </summary>
    /// <param name="item">ItemType being considered for a CraftingItemUI</param>
    /// <returns>The CraftingItemUI to instantiate for the given item</returns>
    private CraftingItemUI GetPrefab(ItemType item)
    {
        return item.craftable ? craftableItemPrefab : resourceItemPrefab;
    }
}