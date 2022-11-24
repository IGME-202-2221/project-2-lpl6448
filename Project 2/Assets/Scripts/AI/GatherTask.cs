using UnityEngine;
using System.Collections.Generic;

public class GatherTask : Task
{
    public ItemType sourceItem;

    private Station destination;

    public GatherTask(ItemType sourceItem)
    {
        this.sourceItem = sourceItem;
    }

    public override float Priority => 0;

    public override Station GetSourceForElf(Elf elf)
    {
        // Find closest station with the given item type as an output item
        Station closestStation = null;
        float closestDisSqr = float.MaxValue;

        foreach (Station station in WorldManager.Instance.stations)
        {
            if (station.CanTakeItem() && station.OutputItem == sourceItem)
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
            if (station.CanReceiveItem(sourceItem, null))
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
            && (source.CanReceiveItem(sourceItem, source) || GetDestinationForElf(elf) != null);
    }

    public override void InitializeTask()
    {
        source.PrepareToTakeItem();
        destination = source.CanReceiveItem(sourceItem, source) ? source : GetDestinationForElf(assignee);
        destination.PrepareToReceiveItem(sourceItem);
    }

    public override void StartTask() { }

    public override void CompleteTask()
    {
        sourceItem = source.TakeItem();
        Taskmaster.Instance.AddTask(new DeliverTask(sourceItem, destination));

        assignee.CarryItem(sourceItem);
    }
}