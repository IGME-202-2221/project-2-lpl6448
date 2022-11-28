using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Task where an item must be delivered to a particular Station.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class DeliverTask : Task
{
    public ItemType deliverItem;

    public DeliverTask(ItemType deliverItem, Station station)
    {
        this.deliverItem = deliverItem;
        SetStation(station);
    }

    public override float Priority => 1;

    public override Vector3 GetStationCenter(Station station) => station.ElfItemCenter;

    public override float GetStationRadius(Station station) => station.elfItemRadius;

    public override float GetProcessingTime() => 1;

    public override bool CanTake(Elf elf)
    {
        return elf.carryingItem == deliverItem && GetSourceForElf(elf) != null;
    }

    public override void InitializeTask() { }

    public override void StartTask() { }

    public override void CompleteTask()
    {
        station.ReceiveItem(deliverItem);

        assignee.DropItem();
    }
}