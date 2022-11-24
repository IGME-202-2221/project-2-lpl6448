using UnityEngine;

public abstract class Station : MonoBehaviour
{
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