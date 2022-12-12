using UnityEngine;

/// <summary>
/// Represents a Station that provides an infinite supply of a particular ingredient item.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class ResourceStation : Station
{
    /// <summary>
    /// ItemType that this Station provides an infinite amount of
    /// </summary>
    public ItemType outputItem;

    /// <summary>
    /// Container/parent Transform of the outputItem's ItemObject
    /// </summary>
    public Transform outputContainer;

    /// <summary>
    /// ItemType that this Station can currently provide to an Elf, containing the outputItem set in the Inspector
    /// </summary>
    public override ItemType OutputItem => outputItem;

    /// <summary>
    /// Determines whether this Station can provide an item to an Elf (always true for a ResourceStation)
    /// </summary>
    /// <returns>Whether this Station can provide an item to an Elf</returns>
    public override bool CanTakeItem() => true;

    /// <summary>
    /// Called when an Elf approaches this Station to take an item
    /// </summary>
    /// <returns>outputItem set in the Inspector to provide to an Elf</returns>
    public override ItemType TakeItem()
    {
        return outputItem;
    }

    /// <summary>
    /// Determines whether this Station can receive the given item from the given source Station.
    /// ResourceStations can never receive items from Elves.
    /// </summary>
    /// <param name="item">ItemType that may be delivered to this Station</param>
    /// <param name="source">Station that provided the item</param>
    /// <returns>Always false for a ResourceStation</returns>
    public override bool CanReceiveItem(ItemType item, Station station) => false;

    /// <summary>
    /// Determines whether this Station can be used by an Elf.
    /// ResourceStations can never be used.
    /// </summary>
    /// <returns>Whether this Station can be used by an Elf</returns>
    public override bool CanUse() => false;

    /// <summary>
    /// To initialize, instantiates the outputItem's ItemObject in the outputContainer
    /// </summary>
    private void Start()
    {
        if (outputItem != null && outputItem.objectPrefab != null)
        {
            ItemObject itemObj = Instantiate(outputItem.objectPrefab, outputContainer, false);
            itemObj.StackObject(0);
        }
    }
}