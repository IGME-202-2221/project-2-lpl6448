using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Task where an item must be gathered from any Station. Before gathering any item,
/// a destination must be determined to ensure that an Elf is not stuck with a useless item.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class GatherTask : Task
{
    /// <summary>
    /// Item that must be collected from any valid Station
    /// </summary>
    public ItemType gatherItem;

    /// <summary>
    /// Station that will receive the gatherItem
    /// </summary>
    private Station destination;

    /// <summary>
    /// Creates a new GatherTask with the given gatherItem
    /// </summary>
    /// <param name="gatherItem">Item that will be collected from any valid Station</param>
    public GatherTask(ItemType gatherItem)
    {
        this.gatherItem = gatherItem;
    }

    /// <summary>
    /// If there are multiple tasks at the same Station, the Priority is used to determine
    /// which Task will be taken by that Elf
    /// </summary>
    public override float Priority => 0;

    /// <summary>
    /// Finds the Station that this Task will be performed at, returning the closest Station to the elf that can
    /// provide the gatherItem.
    /// </summary>
    /// <param name="elf">Elf that is considering taking or has just taken this Task</param>
    /// <returns>Station that the item will be taken from</returns>
    public override Station GetStationForElf(Elf elf)
    {
        // Find closest station with the given item type as an output item
        Station closestStation = null;
        float closestDisSqr = float.MaxValue;

        foreach (Station station in WorldManager.Instance.stations)
        {
            if (station.CanTakeItem() && station.OutputItem == gatherItem)
            {
                float disSqr = (GetStationCenter(station) - elf.physicsObject.Position).sqrMagnitude;
                if (disSqr < closestDisSqr)
                {
                    closestStation = station;
                    closestDisSqr = disSqr;
                }
            }
        }

        return closestStation;
    }

    /// <summary>
    /// Finds a valid Station that can receive the gatherItem, to ensure that the Elf is not
    /// stuck with a useless item after gathering it.
    /// </summary>
    /// <param name="elf">Elf that is considering taking or has just taken this Task</param>
    /// <returns>Station that the gatherItem will eventually be delivered to using a DeliverTask</returns>
    private Station GetDestinationForElf(Elf elf)
    {
        // Find closest station with the given item type as an output item
        Station closestStation = null;
        float closestDisSqr = float.MaxValue;

        foreach (Station station in WorldManager.Instance.stations)
        {
            if (station.CanReceiveItem(gatherItem, null))
            {
                float disSqr = (GetStationCenter(station) - elf.physicsObject.Position).sqrMagnitude;
                if (disSqr < closestDisSqr)
                {
                    closestStation = station;
                    closestDisSqr = disSqr;
                }
            }
        }

        return closestStation;
    }

    /// <summary>
    /// Gets the center of the station's action circle, which an Elf must be touching in order
    /// to complete the Task. The GatherTask's station center is the station's elfItemCenter.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Station's elfItemCenter</returns>
    public override Vector3 GetStationCenter(Station station) => station.ElfItemCenter;

    /// <summary>
    /// Gets the radius of the station's action circle, which an Elf must be touching in order
    /// to complete the Task. The GatherTask's station radius is the station's elfItemRadius.
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
    /// Determines whether this Task can be taken by a particular Elf. An Elf can take a GatherTask
    /// if there is both a valid source Station that can provide the gatherItem and a valid destination
    /// Station that can receive the gatherItem.
    /// </summary>
    /// <param name="elf">Elf that is considering taking this Task</param>
    /// <returns>Whether this Task can be taken by the elf</returns>
    public override bool CanTake(Elf elf)
    {
        Station source = GetStationForElf(elf);
        return elf.carryingItem == null && source != null
            && (source.CanReceiveItem(gatherItem, source) || GetDestinationForElf(elf) != null);
    }

    /// <summary>
    /// Called when this Task is assigned to an Elf, telling the source Station that an item is going to
    /// be collected and telling the destination Station that the item is going to be received
    /// </summary>
    public override void InitializeTask()
    {
        station.PrepareToTakeItem();
        destination = station.CanReceiveItem(gatherItem, station) ? station : GetDestinationForElf(assignee);
        destination.PrepareToReceiveItem(gatherItem);
    }

    /// <summary>
    /// Called when the assignee Elf approaches the Station and starts processing this Task. No additional logic is needed for GatherTask.
    /// </summary>
    public override void StartTask() { }

    /// <summary>
    /// Called after the processing time has been completed by the assignee Elf and this Task has been completed, taking the gatherItem
    /// from the station and starting a new DeliverTask to carry the item to the destination.
    /// </summary>
    public override void CompleteTask()
    {
        gatherItem = station.TakeItem();
        Taskmaster.Instance.AddTask(new DeliverTask(gatherItem, destination));

        assignee.CarryItem(gatherItem);
    }
}