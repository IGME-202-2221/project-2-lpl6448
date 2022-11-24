using UnityEngine;
using System.Collections.Generic;

public class ToyStation : Station
{
    private ItemType outputItem;

    public ItemType pendingBuildItem;

    public List<ItemType> inputItems = new List<ItemType>();

    public List<ItemType> inputItemsInTransit = new List<ItemType>();

    public override ItemType OutputItem => outputItem;

    public override bool CanTakeItem() => outputItem != null;

    public override ItemType TakeItem()
    {
        ItemType item = outputItem;
        outputItem = null;
        return item;
    }

    public override bool CanReceiveItem(ItemType item, Station station)
    {
        if (occupied)
        {
            return false;
        }

        if (outputItem != null && station != this)
        {
            return false;
        }

        if (pendingBuildItem != null)
        {
            List<ItemType> ingredients = new List<ItemType>(pendingBuildItem.ingredients);
            foreach (ItemType ingredient in inputItems)
            {
                ingredients.Remove(ingredient);
            }
            foreach (ItemType ingredient in inputItemsInTransit)
            {
                ingredients.Remove(ingredient);
            }
            
            if (!ingredients.Contains(item))
            {
                return false;
            }
        }

        // If adding the new item would make it so no items can be crafted, do not receive the item
        if (pendingBuildItem == null)
        {
            List<ItemType> inputs = new List<ItemType>();
            inputs.AddRange(inputItems);
            inputs.AddRange(inputItemsInTransit);
            inputs.Add(item);
            HashSet<ItemType> potentialCrafts = FindPotentialCrafts(inputs);
            if (potentialCrafts.Count == 0)
            {
                return false;
            }
        }

        return true;
    }

    public override void PrepareToReceiveItem(ItemType item)
    {
        inputItemsInTransit.Add(item);

        if (pendingBuildItem == null)
        {
            List<ItemType> inputs = new List<ItemType>();
            inputs.AddRange(inputItems);
            inputs.AddRange(inputItemsInTransit);
            HashSet<ItemType> potentialCrafts = FindPotentialCrafts(inputs);
            if (potentialCrafts.Count == 1)
            {
                foreach (ItemType a in potentialCrafts)
                {
                    pendingBuildItem = a;
                    break;
                }
                Taskmaster.Instance.itemsBuilding.Remove(pendingBuildItem);
            }
        }
    }

    public override void ReceiveItem(ItemType item)
    {
        inputItemsInTransit.Remove(item);
        inputItems.Add(item);

        // If we have all of the items to build an item, build it
        if (CanUse())
        {
            Taskmaster.Instance.AddTask(new UseTask(this));
        }
    }

    public override bool CanUse()
    {
        return pendingBuildItem != null && inputItems.Count == pendingBuildItem.ingredients.Length;
    }

    public override void PrepareToUse()
    {
        // ?
    }

    public override void BeginUse()
    {
        occupied = true;
    }

    public override void EndUse()
    {
        occupied = false;
        outputItem = pendingBuildItem;
        inputItems.Clear();
        pendingBuildItem = null;

        Taskmaster.Instance.FinishBuildingItem(outputItem);

        GatherTask newTask = new GatherTask(outputItem);
        newTask.SetSource(this);
        Taskmaster.Instance.AddTask(newTask);
    }

    private HashSet<ItemType> FindPotentialCrafts(List<ItemType> inputs)
    {
        HashSet<ItemType> craftedItems = new HashSet<ItemType>();
        foreach (ItemType craftedItem in Taskmaster.Instance.itemsBuilding)
        {
            // If there are more inputs than ingredients, this cannot be a potential craft
            if (inputs.Count > craftedItem.ingredients.Length)
            {
                continue;
            }

            // Each item in the inputs list must be in the ingredients for it to be a potential craft
            List<ItemType> ingredients = new List<ItemType>(craftedItem.ingredients);
            bool isPotentialCraft = true;
            foreach (ItemType ingredient in inputs)
            {
                if (!ingredients.Remove(ingredient))
                {
                    isPotentialCraft = false;
                    break;
                }
            }

            if (isPotentialCraft)
            {
                craftedItems.Add(craftedItem);
            }
        }
        return craftedItems;
    }

    private List<ItemType> FindValidCrafts(List<ItemType> inputs)
    {
        List<ItemType> craftedItems = new List<ItemType>();
        foreach (ItemType craftedItem in Taskmaster.Instance.itemsBuilding)
        {
            // If there are more or fewer inputs than ingredients, this cannot be a valid craft
            if (inputs.Count != craftedItem.ingredients.Length)
            {
                continue;
            }

            // Each item in the inputs list must be in the ingredients for it to be a potential craft,
            // and all ingredients must be fulfilled for it to be a valid craft
            List<ItemType> ingredients = new List<ItemType>(craftedItem.ingredients);
            bool isPotentialCraft = true;
            foreach (ItemType ingredient in inputs)
            {
                if (!ingredients.Remove(ingredient))
                {
                    isPotentialCraft = false;
                    break;
                }
            }

            if (isPotentialCraft)
            {
                craftedItems.Add(craftedItem);
            }
        }
        return craftedItems;
    }
}