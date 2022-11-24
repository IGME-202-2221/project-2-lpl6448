using UnityEngine;
using System.Collections.Generic;

public class DeliverTask : Task
{
    public ItemType sourceItem;

    public DeliverTask(ItemType sourceItem, Station source)
    {
        this.sourceItem = sourceItem;
        this.SetSource(source);
    }

    public override float Priority => 1;

    public override Vector3 GetStationCenter(Station station) => station.ElfItemCenter;

    public override float GetStationRadius(Station station) => station.elfItemRadius;

    public override float GetProcessingTime() => 1;

    public override bool CanTake(Elf elf)
    {
        return elf.carryingItem == sourceItem && GetSourceForElf(elf) != null;
    }

    public override void InitializeTask() { }

    public override void StartTask() { }

    public override void CompleteTask()
    {
        source.ReceiveItem(sourceItem);

        assignee.DropItem();
    }
}