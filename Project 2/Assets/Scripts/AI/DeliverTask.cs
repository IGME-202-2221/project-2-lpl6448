using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Task where an item must be delivered to a particular Station.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class DeliverTask : Task
{
    /// <summary>
    /// Item that will be received by the station
    /// </summary>
    public ItemType deliverItem;

    /// <summary>
    /// Creates a new DeliverTask with the given ItemType to deliver and Station to deliver it to
    /// </summary>
    /// <param name="deliverItem">Item that will be received by the station</param>
    /// <param name="station">Station to deliver the deliverItem to</param>
    public DeliverTask(ItemType deliverItem, Station station)
    {
        this.deliverItem = deliverItem;
        SetStation(station);
    }

    /// <summary>
    /// If there are multiple tasks at the same Station, the Priority is used to determine
    /// which Task will be taken by that Elf
    /// </summary>
    public override float Priority => 1;

    /// <summary>
    /// Gets the center of the station's action circle, which an Elf must be touching in order
    /// to complete the Task. The DeliverTask's station center is the station's elfItemCenter.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Station's elfItemCenter</returns>
    public override Vector3 GetStationCenter(Station station) => station.ElfItemCenter;

    /// <summary>
    /// Gets the radius of the station's action circle, which an Elf must be touching in order
    /// to complete the Task. The DeliverTask's station radius is the station's elfItemRadius.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Station's elfItemRadius</returns>
    public override float GetStationRadius(Station station) => station.elfItemRadius;

    /// <summary>
    /// Gets the number of seconds for an Elf to complete this Task, after approaching the station
    /// </summary>
    /// <returns>Number of seconds for an Elf to complete this Task</returns>
    public override float GetProcessingTime() => 1;

    /// <summary>
    /// Determines whether this Task can be taken by a particular Elf.
    /// </summary>
    /// <param name="elf">Elf that is considering taking this Task</param>
    /// <returns>Whether the elf is carrying the item that must be delivered and whether the station is valid</returns>
    public override bool CanTake(Elf elf)
    {
        return elf.carryingItem == deliverItem && GetStationForElf(elf) != null;
    }

    /// <summary>
    /// Called when this Task is assigned to an Elf. No additional logic is needed for DeliverTask.
    /// </summary>
    public override void InitializeTask() { }

    /// <summary>
    /// Called when the assignee Elf approaches the Station and starts processing this Task. No additional logic is needed for DeliverTask.
    /// </summary>
    public override void StartTask() { }

    /// <summary>
    /// Called after the processing time has been completed by the assignee Elf and this Task has been completed,
    /// taking the item from the assignee Elf and delivering it to the station
    /// </summary>
    public override void CompleteTask()
    {
        station.ReceiveItem(deliverItem);

        assignee.DropItem();
    }
}