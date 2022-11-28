using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Task where an item must be gathered from any Station. Before gathering any item,
/// a destination must be determined to ensure that an Elf is not stuck with a useless item.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class GatherTask : Task
{
    public ItemType gatherItem;

    private Station destination;

    public GatherTask(ItemType gatherItem)
    {
        this.gatherItem = gatherItem;
    }

    public override float Priority => 0;

    public override Station GetSourceForElf(Elf elf)
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

    public override Vector3 GetStationCenter(Station station) => station.ElfItemCenter;

    public override float GetStationRadius(Station station) => station.elfItemRadius;

    public override float GetProcessingTime() => 1;

    public override bool CanTake(Elf elf)
    {
        Station source = GetSourceForElf(elf);
        return elf.carryingItem == null && source != null
            && (source.CanReceiveItem(gatherItem, source) || GetDestinationForElf(elf) != null);
    }

    public override void InitializeTask()
    {
        station.PrepareToTakeItem();
        destination = station.CanReceiveItem(gatherItem, station) ? station : GetDestinationForElf(assignee);
        destination.PrepareToReceiveItem(gatherItem);
    }

    public override void StartTask() { }

    public override void CompleteTask()
    {
        gatherItem = station.TakeItem();
        Taskmaster.Instance.AddTask(new DeliverTask(gatherItem, destination));

        assignee.CarryItem(gatherItem);
    }
}