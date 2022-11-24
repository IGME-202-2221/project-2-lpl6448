using UnityEngine;

public class ResourceStation : Station
{
    public ItemType outputItem;

    public override ItemType OutputItem => outputItem;

    public override bool CanTakeItem() => true;

    public override ItemType TakeItem()
    {
        return outputItem;
    }

    public override bool CanReceiveItem(ItemType item, Station station) => false;

    public override bool CanUse() => false;
}