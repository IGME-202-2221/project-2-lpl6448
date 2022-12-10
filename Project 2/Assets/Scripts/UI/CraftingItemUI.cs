using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingItemUI : MonoBehaviour
{
    public RectTransform contentAreaTransform;

    public ItemType item;

    public TextMeshProUGUI itemNameText;

    public Image itemIcon;

    public TextMeshProUGUI itemMultiplierText;

    public Button craftButton;

    public Button expandButton;

    public List<CraftingItemUI> nestedItems;

    public float indentFactor = 60;

    private bool expanded;

    private bool canCraft;

    public void SetCanCraft(bool canCraft)
    {
        this.canCraft = canCraft;
        craftButton.interactable = canCraft;
    }

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

    public void Collapse()
    {
        expanded = false;
        expandButton.transform.localEulerAngles = new Vector3(0, 0, 90);
        foreach (CraftingItemUI item in nestedItems)
        {
            item.Disable();
        }
    }

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

    private void Disable()
    {
        gameObject.SetActive(false);
        foreach (CraftingItemUI item in nestedItems)
        {
            item.Disable();
        }
    }

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

    public void CraftItem()
    {
        if (canCraft)
        {
            UIManager.Instance.craftingPanel.EnterCraftMode(this);
        }
    }
}