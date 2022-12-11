using UnityEngine;

/// <summary>
/// Represents a Station that accepts fully crafted items and registers their completion.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class FinishStation : Station
{
    public LaunchPad launchPadPrefab;

    public Transform launchPadContainer;

    public float secondsToReplaceLaunchPad;

    private float lastLaunchTime;

    private LaunchPad currentLaunchPad;

    public override ItemType OutputItem => null;

    public override bool CanTakeItem() => false;

    public override bool CanReceiveItem(ItemType item, Station station)
    {
        return Taskmaster.Instance.requestedItems.Contains(item);
    }

    public override void PrepareToReceiveItem(ItemType item)
    {
        Taskmaster.Instance.requestedItems.Remove(item);
    }

    public override void ReceiveItem(ItemType item)
    {
        if (currentLaunchPad != null)
        {
            Destroy(currentLaunchPad.gameObject);
        }

        LaunchPad newLaunchPad = Instantiate(launchPadPrefab, launchPadContainer, false);
        newLaunchPad.AttachItem(item);
        newLaunchPad.Launch();

        lastLaunchTime = Time.time;
    }

    public override bool CanUse() => false;

    private void Start()
    {
        AttemptRespawnLaunchPad();
    }

    private void Update()
    {
        if (Time.time - lastLaunchTime >= secondsToReplaceLaunchPad)
        {
            AttemptRespawnLaunchPad();
        }
    }

    private void AttemptRespawnLaunchPad()
    {
        if (currentLaunchPad == null)
        {
            currentLaunchPad = Instantiate(launchPadPrefab, launchPadContainer, false);
        }
    }
}