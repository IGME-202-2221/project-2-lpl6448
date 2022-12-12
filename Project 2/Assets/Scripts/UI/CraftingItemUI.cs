using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Represents the UI element for a specific crafting or resource item, which displays information
/// about the item, such as name, icon, and craftability.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class CraftingItemUI : MonoBehaviour
{
    /// <summary>
    /// ItemType that this CraftingItemUI displays information about
    /// </summary>
    public ItemType item;

    /// <summary>
    /// RectTransform that contains information about the item, used to indent
    ///     ingredient or resource items forward.
    /// </summary>
    public RectTransform contentAreaTransform;

    /// <summary>
    /// TextMeshPro element that contains the name of the item
    /// </summary>
    public TextMeshProUGUI itemNameText;

    /// <summary>
    /// Image containing the item's icon
    /// </summary>
    public Image itemIcon;

    /// <summary>
    /// TestMeshPro element containing the number of this item that are required for crafting
    /// </summary>
    public TextMeshProUGUI itemMultiplierText;

    /// <summary>
    /// Button that enters Crafting mode based on this item when clicked
    /// </summary>
    public Button craftButton;

    /// <summary>
    /// Button that expands or collapses ingredient items
    /// </summary>
    public Button expandButton;

    /// <summary>
    /// List of nested (or ingredient) CraftingItemUIs that are activated/deactived when expanding/collapsing
    /// </summary>
    public List<CraftingItemUI> nestedItems;

    /// <summary>
    /// Number of (scaled) pixels to indent the contentAreaTransform by per level
    /// </summary>
    public float indentFactor = 60;

    /// <summary>
    /// Whether this CraftingItemUI is currently expanded (whether its nestedItems are visible)
    /// </summary>
    private bool expanded;

    /// <summary>
    /// Whether the item can currently be crafted
    /// </summary>
    private bool canCraft;

    /// <summary>
    /// Updates the canCraft field and the interactability of the craftButton
    /// </summary>
    /// <param name="canCraft">Whether the item can currently be crafted</param>
    public void SetCanCraft(bool canCraft)
    {
        this.canCraft = canCraft;
        craftButton.interactable = canCraft;
    }

    /// <summary>
    /// Initializes this CraftingItemUI, displaying the given item, indent, and multiplier
    /// </summary>
    /// <param name="item">ItemType that this CraftingItemUI will contain information about</param>
    /// <param name="indent">Number of indents/levels to apply to the contentAreaTransform</param>
    /// <param name="multiplier">Number of the item that is required for crafting</param>
    public void InitializeItem(ItemType item, int indent, int multiplier)
    {
        this.item = item;
        contentAreaTransform.offsetMin = new Vector2(indent * indentFactor, contentAreaTransform.offsetMin.y);
        itemNameText.text = item.displayName;

        if (item.icon != null)
        {
            itemIcon.sprite = item.icon;
        }
        else
        {
            itemIcon.enabled = false;
        }

        if (multiplier == 1)
        {
            itemMultiplierText.text = "";
        }
        else
        {
            itemMultiplierText.text = "x" + multiplier;
        }
    }

    /// <summary>
    /// Collapses this CraftingItemUI, disabling all nestedItems
    /// </summary>
    public void Collapse()
    {
        expanded = false;
        expandButton.transform.localEulerAngles = new Vector3(0, 0, 90);
        foreach (CraftingItemUI item in nestedItems)
        {
            item.Disable();
        }
    }

    /// <summary>
    /// Expands this CraftingItemUI, enabling all nestedItems and expanding any
    /// that were previously expanded
    /// </summary>
    public void Expand()
    {
        expanded = true;
        expandButton.transform.localEulerAngles = new Vector3(0, 0, 0);
        foreach (CraftingItemUI item in nestedItems)
        {
            item.gameObject.SetActive(true);
            if (item.expanded)
            {
                item.Expand();
            }
        }
    }

    /// <summary>
    /// Deactivates this CraftingItemUI and recursively deactivates all nestedItems
    /// </summary>
    private void Disable()
    {
        gameObject.SetActive(false);
        foreach (CraftingItemUI item in nestedItems)
        {
            item.Disable();
        }
    }

    /// <summary>
    /// Toggles the expanded field and expands or collapes this CraftingItemUI as necessary
    /// </summary>
    public void ToggleExpanded()
    {
        expanded = !expanded;
        if (expanded)
        {
            Expand();
        }
        else
        {
            Collapse();
        }
    }

    /// <summary>
    /// If the item can currently be crafted, enters Crafting mode through the CraftingPanel
    /// </summary>
    public void CraftItem()
    {
        if (canCraft)
        {
            UIManager.Instance.craftingPanel.EnterCraftMode(this);
        }
    }
}