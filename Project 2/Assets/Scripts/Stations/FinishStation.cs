using UnityEngine;

/// <summary>
/// Represents a Station that accepts fully crafted items and registers their completion.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class FinishStation : Station
{
    /// <summary>
    /// LaunchPad prefab that is instantiated to launch completed items to Santa
    /// </summary>
    public LaunchPad launchPadPrefab;

    /// <summary>
    /// Container/parent for any instantiated LaunchPads
    /// </summary>
    public Transform launchPadContainer;

    /// <summary>
    /// Number of seconds after beginning to launch an item before a new LaunchPad is instantiated to replace
    /// the old LaunchPad
    /// </summary>
    public float secondsToReplaceLaunchPad;

    /// <summary>
    /// In-game time when the last item was launched, used to replace old LaunchPads with new ones
    /// </summary>
    private float lastLaunchTime;

    /// <summary>
    /// Reference to the current empty LaunchPad, waiting to launch an item
    /// </summary>
    private LaunchPad currentLaunchPad;

    /// <summary>
    /// ItemType that this Station can currently provide to an Elf. FinishStations cannot provide any items.
    /// </summary>
    public override ItemType OutputItem => null;

    /// <summary>
    /// Determines whether this Station can provide an item to an Elf. FinishStations cannot provide any items.
    /// </summary>
    /// <returns>Always false for a FinishStation</returns>
    public override bool CanTakeItem() => false;

    /// <summary>
    /// Determines whether this Station can receive the given item from the given source Station. A FinishStation
    /// can receive an item if it has been requested by the Taskmaster.
    /// </summary>
    /// <param name="item">ItemType that may be delivered to this Station</param>
    /// <param name="source">Station that provided the item</param>
    /// <returns>Whether this Station can receive the item from the source Station</returns>
    public override bool CanReceiveItem(ItemType item, Station station)
    {
        return Taskmaster.Instance.requestedItems.Contains(item);
    }

    /// <summary>
    /// Called when a Task determines that this Station will receive an item from an Elf, telling the Taskmaster
    /// that this item has been crafted and is no longer requested
    /// </summary>
    /// <param name="item">ItemType that will be received by this Station</param>
    public override void PrepareToReceiveItem(ItemType item)
    {
        Taskmaster.Instance.requestedItems.Remove(item);
    }

    /// <summary>
    /// Called when an Elf approaches this Station to give it an item, destroying the currentLaunchPad and
    /// instantiating and launching a new one with the given item attached to it
    /// </summary>
    /// <param name="item">ItemType that this Station receives from the Elf</param>
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

    /// <summary>
    /// Determines whether this Station can be used by an Elf. FinishStations can never be used by Elves.
    /// </summary>
    /// <returns>Always false for a FinishStation</returns>
    public override bool CanUse() => false;

    /// <summary>
    /// To initialize, spawns a new empty LaunchPad
    /// </summary>
    private void Start()
    {
        AttemptRespawnLaunchPad();
    }

    /// <summary>
    /// Every frame, if the time since the last launch exceeds a given number of seconds,
    /// creates a new LaunchPad if necessary
    /// </summary>
    private void Update()
    {
        if (Time.time - lastLaunchTime >= secondsToReplaceLaunchPad)
        {
            AttemptRespawnLaunchPad();
        }
    }

    /// <summary>
    /// Instantiates a new empty LaunchPad if the current one has been launched
    /// </summary>
    private void AttemptRespawnLaunchPad()
    {
        if (currentLaunchPad == null)
        {
            currentLaunchPad = Instantiate(launchPadPrefab, launchPadContainer, false);
        }
    }
}