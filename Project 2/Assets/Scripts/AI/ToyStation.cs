using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Station where toys/items can be constructed from ingredient items.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class ToyStation : Station
{
    private ItemType outputItem;

    private bool outputItemBeingTaken = false;

    public Transform stackedContainer;

    public Transform outputContainer;

    private Stack<ItemObject> stackedItems = new Stack<ItemObject>();

    private float stackedItemsHeight = 0;

    private ItemObject outputItemObj;

    public ItemType pendingBuildItem;

    public List<ItemType> inputItems = new List<ItemType>();

    public List<ItemType> inputItemsInTransit = new List<ItemType>();

    public override ItemType OutputItem => outputItem;

    public override bool CanTakeItem() => outputItem != null && !outputItemBeingTaken;

    public override void PrepareToTakeItem()
    {
        outputItemBeingTaken = true;
    }

    public override ItemType TakeItem()
    {
        ItemType item = outputItem;
        outputItem = null;
        outputItemBeingTaken = false;
        stationCanvas.SetItem(pendingBuildItem);
        if (outputItemObj != null)
        {
            Destroy(outputItemObj.gameObject);
            outputItemObj = null;
        }
        return item;
    }

    public override bool CanReceiveItem(ItemType item, Station station)
    {
        // If an item is not currently being built at this station or it is already
        // being used, it cannot receive any input items
        if (occupied || pendingBuildItem == null)
        {
            return false;
        }

        // A station with an output item cannot receive any input items
        // (unless the output item is being moved to the same station)
        if (outputItem != null && station != this)
        {
            return false;
        }

        // If the input item is not required for the pending item's recipe, do not accept it
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

        return true;
    }

    public override void PrepareToReceiveItem(ItemType item)
    {
        inputItemsInTransit.Add(item);
    }

    public override void ReceiveItem(ItemType item)
    {
        inputItemsInTransit.Remove(item);
        inputItems.Add(item);

        StackItem(item);

        // If we have all of the items to build an item, build it
        if (CanUse())
        {
            Taskmaster.Instance.AddTask(new UseTask(this));
        }
    }

    public override bool CanUse()
    {
        return pendingBuildItem != null && inputItems.Count == pendingBuildItem.ingredients.Count;
    }

    public override void BeginUse()
    {
        occupied = true;
    }

    public override string UsingAnimation => "Hammering";

    public override void EndUse()
    {
        occupied = false;
        outputItem = pendingBuildItem;
        inputItems.Clear();
        pendingBuildItem = null;

        outputItemObj = Instantiate(outputItem.objectPrefab, outputContainer, false);
        outputItemObj.StackObject(0);

        while (stackedItems.Count > 0)
        {
            UnstackItem();
        }

        Taskmaster.Instance.FinishBuildingItem(outputItem);

        //GatherTask newTask = new GatherTask(outputItem);
        //newTask.SetStation(this);
        //Taskmaster.Instance.AddTask(newTask);
    }

    public override bool CanAcceptUserItem(ItemType item)
    {
        // If we are currently building an item, do not accept
        if (pendingBuildItem != null || occupied)
        {
            return false;
        }

        // If this station has an output item, do not accept unless it is an ingredient for the user item
        if (outputItem != null && !item.ingredients.Contains(outputItem))
        {
            return false;
        }

        return true;
    }

    public override void AcceptUserItem(ItemType item)
    {
        pendingBuildItem = item;
        stationCanvas.SetItem(item);

        // Initialize tasks to collect the necessary items
        foreach (ItemType ingredient in item.ingredients)
        {
            Taskmaster.Instance.AddTask(new GatherTask(ingredient));
        }
    }

    private void StackItem(ItemType item)
    {
        ItemObject itemObj = Instantiate(item.objectPrefab, stackedContainer, false);
        itemObj.StackObject(stackedItemsHeight);

        stackedItemsHeight += itemObj.stackedHeight;
        stackedItems.Push(itemObj);
    }

    private void UnstackItem()
    {
        ItemObject itemObj = stackedItems.Pop();

        stackedItemsHeight -= itemObj.stackedHeight;
        Destroy(itemObj.gameObject);
    }

    private void Start()
    {
        stationCanvas.SetItem(null);
    }
}