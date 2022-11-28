using UnityEngine;

/// <summary>
/// Represents a Station that accepts fully crafted items and registers their completion.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class ConveyorStation : Station
{
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    public override ItemType OutputItem => null;

    public override bool CanTakeItem() => false;

    public override bool CanReceiveItem(ItemType item, Station station)
    {
        return Taskmaster.Instance.initItems.Contains(item);
    }

    public override void PrepareToReceiveItem(ItemType item)
    {
        Taskmaster.Instance.initItems.Remove(item);
    }

    public override void ReceiveItem(ItemType item)
    {
        print(item + " delivered");
        if (Taskmaster.Instance.initItems.Count == 0)
        {
            print("Finished! Elapsed time: " + (Time.time - startTime) + "s");
        }
    }

    public override bool CanUse() => false;
}