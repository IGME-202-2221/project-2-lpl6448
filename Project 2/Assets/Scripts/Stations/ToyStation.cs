using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Station where toys/items can be constructed from ingredient items.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class ToyStation : Station
{
    /// <summary>
    /// ItemType that this ToyStation has just crafted (if it has not been taken by an Elf yet)
    /// </summary>
    private ItemType outputItem;

    /// <summary>
    /// Whether the outputItem is going to be taken by an Elf soon
    /// </summary>
    private bool outputItemBeingTaken = false;

    /// <summary>
    /// Container/parent Transform for stacked input ItmeObjects
    /// </summary>
    public Transform stackedContainer;

    /// <summary>
    /// Container/parent Transform for the outputItem's ItemObject
    /// </summary>
    public Transform outputContainer;

    /// <summary>
    /// ItemType that this ToyStation is currently accepted input items to craft
    /// </summary>
    public ItemType pendingBuildItem;

    /// <summary>
    /// List of ItemTypes that this ToyStation is using to craft the pendingBuildItem
    /// </summary>
    public List<ItemType> inputItems = new List<ItemType>();

    /// <summary>
    /// List of ItemTypes that are in the process of being delivered by Elves to this ToyStation
    /// </summary>
    public List<ItemType> inputItemsInTransit = new List<ItemType>();

    /// <summary>
    /// Stack containing the input ItemObjects, used to easily add and remove
    /// ItemObjects from the game's literal stack of items
    /// </summary>
    private Stack<ItemObject> stackedItems = new Stack<ItemObject>();

    /// <summary>
    /// Cumulative height of all stacked input ItemObjects
    /// </summary>
    private float stackedItemsHeight = 0;

    /// <summary>
    /// Current ItemObject of the outputItem (or null if the outputItem slot is empty)
    /// </summary>
    private ItemObject outputItemObj;

    /// <summary>
    /// ItemType that this Station can currently provide to an Elf. For a ToyStation, this is
    /// the crafted outputItem, or null if no item has been crafted yet.
    /// </summary>
    public override ItemType OutputItem => outputItem;

    /// <summary>
    /// Determines whether this Station can provide an item to an Elf. A ToyStation can provide an item
    /// if there is an item in the outputItem slot that is not being taken by another Elf.
    /// </summary>
    /// <returns>Whether this Station can provide an item to an Elf</returns>
    public override bool CanTakeItem() => outputItem != null && !outputItemBeingTaken;

    /// <summary>
    /// Called when a Task determines that this Station will provide an item to an Elf,
    /// telling this ToyStation not to let any other Elves take the outputItem
    /// </summary>
    public override void PrepareToTakeItem()
    {
        outputItemBeingTaken = true;
    }

    /// <summary>
    /// Called when an Elf approaches this Station to take an item, removing the outputItem,
    /// destroying the outputItemObj, and updating the stationCanvas
    /// </summary>
    /// <returns>ItemType that is provided to the Elf</returns>
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

    /// <summary>
    /// Determines whether this Station can receive the given item from the given source Station. A ToyStation
    /// can receive an item if it is currently building an item and the item to receive is a remaining ingredient
    /// for the pendingBuildItem
    /// </summary>
    /// <param name="item">ItemType that may be delivered to this Station</param>
    /// <param name="source">Station that provided the item</param>
    /// <returns>Whether this Station can receive the item from the source Station</returns>
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

    /// <summary>
    /// Called when a Task determines that this Station will receive an item from an Elf, adding the
    /// item to inputItemsInTransit
    /// </summary>
    /// <param name="item">ItemType that will be received by this Station</param>
    public override void PrepareToReceiveItem(ItemType item)
    {
        inputItemsInTransit.Add(item);
    }

    /// <summary>
    /// Called when an Elf approaches this Station to give it an item, adding the item to the inputItems,
    /// stacking its ItemObject visually, and creating a UseTask if the pendingBuildItem has all of its ingredients
    /// </summary>
    /// <param name="item">ItemType that this Station receives from the Elf</param>
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

    /// <summary>
    /// Determines whether this Station can be used by an Elf. A ToyStation can be used by an Elf if
    /// it is currently crafting an item and all of the ingredients are present
    /// </summary>
    /// <returns>Whether this Station can be used by an Elf</returns>
    public override bool CanUse()
    {
        return pendingBuildItem != null && inputItems.Count == pendingBuildItem.ingredients.Count;
    }

    /// <summary>
    /// Called when a Task determines that this Station will be used by an Elf, registering that
    /// this ToyStation is now occupied
    /// </summary>
    public override void BeginUse()
    {
        occupied = true;
    }

    /// <summary>
    /// Called when the Elf is done using this Station (after the Task's processing time), resetting
    /// the ToyStation and telling the Taskmaster that the item has been completed
    /// </summary>
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
    }

    /// <summary>
    /// Animator string that will be enabled when this Station is being used, making the Elves
    /// pull out their hammers while using a ToyStation
    /// </summary>
    public override string UsingAnimation => "Hammering";

    /// <summary>
    /// Determines whether this Station can accept an item from the CraftingPanel. A ToyStation can accept a user item
    /// if it is not currently building any item and does not currently have an outputItem to get rid of
    /// </summary>
    /// <param name="item">ItemType that may be accepted by this Station</param>
    /// <returns>Whether this Station can accept an item from the CraftingPanel (false by default)</returns>
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

    /// <summary>
    /// Accepts the item from the CraftingPanel, updating the pendingBuildItem and the stationCanvas
    /// and creating GatherTasks to collect ingredients for the new item
    /// </summary>
    /// <param name="item">ItemType that is accepted by this Station</param>
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

    /// <summary>
    /// Instantiates the item's ItemObject and placing it on top of the other stacked input ItemObjects
    /// </summary>
    /// <param name="item">ItemType to stack</param>
    private void StackItem(ItemType item)
    {
        ItemObject itemObj = Instantiate(item.objectPrefab, stackedContainer, false);
        itemObj.StackObject(stackedItemsHeight);

        stackedItemsHeight += itemObj.stackedHeight;
        stackedItems.Push(itemObj);
    }

    /// <summary>
    /// Removes and destroys the top ItemObject on the top of the input ItemObjects
    /// </summary>
    private void UnstackItem()
    {
        ItemObject itemObj = stackedItems.Pop();

        stackedItemsHeight -= itemObj.stackedHeight;
        Destroy(itemObj.gameObject);
    }

    /// <summary>
    /// Initializes the stationCanvas with no pendingBuildItem
    /// </summary>
    private void Start()
    {
        stationCanvas.SetItem(null);
    }
}