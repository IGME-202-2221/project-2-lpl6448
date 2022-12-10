using UnityEngine;
using System.Collections.Generic;

public class CraftingPanel : MonoBehaviour
{
    public Transform itemsContainer;

    public CraftingItemUI craftableItemPrefab;

    public CraftingItemUI resourceItemPrefab;

    private List<CraftingItemUI> rootItemsUI = new List<CraftingItemUI>();

    private bool craftMode = false;

    private CraftingItemUI craftItemUI = null;

    public void Populate(List<ItemType> rootItems)
    {
        foreach (ItemType rootItem in rootItems)
        {
            rootItemsUI.Add(CreateItemUI(rootItem, 0, 1));
        }
        RefreshCanBuild();
    }

    public void RefreshCanBuild()
    {
        foreach (CraftingItemUI itemUI in rootItemsUI)
        {
            RefreshCanBuild(itemUI);
        }
    }

    private void RefreshCanBuild(CraftingItemUI itemUI)
    {
        itemUI.SetCanCraft(itemUI.item.craftable && Taskmaster.Instance.CanBuildItem(itemUI.item));

        foreach (CraftingItemUI nestedItem in itemUI.nestedItems)
        {
            RefreshCanBuild(nestedItem);
        }
    }

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

    public void ExitCraftMode()
    {
        craftMode = false;
        craftItemUI = null;
    }

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

    private CraftingItemUI GetPrefab(ItemType item)
    {
        return item.craftable ? craftableItemPrefab : resourceItemPrefab;
    }
}