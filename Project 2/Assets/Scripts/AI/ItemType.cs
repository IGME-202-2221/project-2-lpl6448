using UnityEngine;

[CreateAssetMenu(fileName = "New ItemType", menuName = "ItemType")]
public class ItemType : ScriptableObject
{
    public string displayName;

    public GameObject carryPrefab;

    public bool craftable;

    public ItemType[] ingredients;
}