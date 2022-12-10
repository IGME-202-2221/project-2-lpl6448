using UnityEngine;

/// <summary>
/// Represents a target for Elves that can either receive/take items from Elves,
/// provide items to Elves, and be used by Elves.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public abstract class Station : MonoBehaviour
{
    public StationCanvas stationCanvas;

    public Transform elfItemCenter;

    public float elfItemRadius;

    public Transform elfUseCenter;

    public float elfUseRadius;

    public Vector3 ElfItemCenter => elfItemCenter.position;

    public Vector3 ElfUseCenter => elfUseCenter.position;


    public bool occupied { get; protected set; }

    public abstract ItemType OutputItem { get; }

    public abstract bool CanTakeItem();

    public virtual void PrepareToTakeItem() { }

    public virtual ItemType TakeItem() => null;

    public abstract bool CanReceiveItem(ItemType item, Station source);

    public virtual void PrepareToReceiveItem(ItemType item) { }

    public virtual void ReceiveItem(ItemType item) { }

    public abstract bool CanUse();

    public virtual void PrepareToUse() { }

    public virtual void BeginUse() { }

    public virtual void EndUse() { }

    public virtual string UsingAnimation => "";

    public virtual bool CanAcceptUserItem(ItemType item) => false;

    public virtual void AcceptUserItem(ItemType item) { }


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