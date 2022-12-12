using UnityEngine;

/// <summary>
/// Represents a target for Elves that can either receive/take items from Elves,
/// provide items to Elves, and be used by Elves.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public abstract class Station : MonoBehaviour
{
    /// <summary>
    /// Reference to the StationCanvas that displays information about this Station
    /// </summary>
    public StationCanvas stationCanvas;

    /// <summary>
    /// Positional reference for the center of the action circle for GatherTasks and DeliverTasks
    /// </summary>
    public Transform elfItemCenter;

    /// <summary>
    /// Radius of the action circle for GatherTasks and DeliverTasks
    /// </summary>
    public float elfItemRadius;

    /// <summary>
    /// Positional reference for teh center of the action circle for UseTasks
    /// </summary>
    public Transform elfUseCenter;

    /// <summary>
    /// Radius of the action circle for UseTasks
    /// </summary>
    public float elfUseRadius;

    /// <summary>
    /// Center of the action circle for GatherTasks and DeliverTasks
    /// </summary>
    public Vector3 ElfItemCenter => elfItemCenter.position;

    /// <summary>
    /// Center of the action circle for UseTasks
    /// </summary>
    public Vector3 ElfUseCenter => elfUseCenter.position;
    
    /// <summary>
    /// Whether this Station is currently being used
    /// </summary>
    public bool occupied { get; protected set; }

    /// <summary>
    /// ItemType that this Station can currently provide to an Elf
    /// </summary>
    public abstract ItemType OutputItem { get; }

    /// <summary>
    /// Determines whether this Station can provide an item to an Elf
    /// </summary>
    /// <returns>Whether this Station can provide an item to an Elf</returns>
    public abstract bool CanTakeItem();

    /// <summary>
    /// Called when a Task determines that this Station will provide an item to an Elf
    /// </summary>
    public virtual void PrepareToTakeItem() { }

    /// <summary>
    /// Called when an Elf approaches this Station to take an item
    /// </summary>
    /// <returns>ItemType that is provided to the Elf</returns>
    public virtual ItemType TakeItem() => null;

    /// <summary>
    /// Determines whether this Station can receive the given item from the given source Station
    /// </summary>
    /// <param name="item">ItemType that may be delivered to this Station</param>
    /// <param name="source">Station that provided the item</param>
    /// <returns>Whether this Station can receive the item from the source Station</returns>
    public abstract bool CanReceiveItem(ItemType item, Station source);

    /// <summary>
    /// Called when a Task determines that this Station will receive an item from an Elf
    /// </summary>
    /// <param name="item">ItemType that will be received by this Station</param>
    public virtual void PrepareToReceiveItem(ItemType item) { }

    /// <summary>
    /// Called when an Elf approaches this Station to give it an item
    /// </summary>
    /// <param name="item">ItemType that this Station receives from the Elf</param>
    public virtual void ReceiveItem(ItemType item) { }

    /// <summary>
    /// Determines whether this Station can be used by an Elf
    /// </summary>
    /// <returns>Whether this Station can be used by an Elf</returns>
    public abstract bool CanUse();

    /// <summary>
    /// Called when a Task determines that this Station will be used by an Elf
    /// </summary>
    public virtual void PrepareToUse() { }

    /// <summary>
    /// Called when an Elf approaches this Station and begins using it
    /// </summary>
    public virtual void BeginUse() { }

    /// <summary>
    /// Called when the Elf is done using this Station (after the Task's processing time)
    /// </summary>
    public virtual void EndUse() { }

    /// <summary>
    /// Animator string that will be enabled when this Station is being used, allowing for
    /// each Station to have a different using animation
    /// </summary>
    public virtual string UsingAnimation => "";

    /// <summary>
    /// Determines whether this Station can accept an item from the CraftingPanel
    /// </summary>
    /// <param name="item">ItemType that may be accepted by this Station</param>
    /// <returns>Whether this Station can accept an item from the CraftingPanel (false by default)</returns>
    public virtual bool CanAcceptUserItem(ItemType item) => false;

    /// <summary>
    /// Accepts the item from the CraftingPanel
    /// </summary>
    /// <param name="item">ItemType that is accepted by this Station</param>
    public virtual void AcceptUserItem(ItemType item) { }

    /// <summary>
    /// When selected, draws blue and red wire spheres to represent the item and use action circles, respectively
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (elfItemCenter != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ElfItemCenter, elfItemRadius);
        }

        if (elfUseCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ElfUseCenter, elfUseRadius);
        }
    }
}