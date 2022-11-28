using UnityEngine;

/// <summary>
/// Represents a type of craftable or gatherable item in the game and includes
/// the information necessary to instantiate it.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
[CreateAssetMenu(fileName = "New ItemType", menuName = "ItemType")]
public class ItemType : ScriptableObject
{
    /// <summary>
    /// Display name for this item, which will be displayed in the game UI
    /// </summary>
    public string displayName;

    /// <summary>
    /// Prefab that is instantiated as a child of the Elf's CarryAnchor object to show that the Elf is carrying this item
    /// </summary>
    public GameObject carryPrefab;

    /// <summary>
    /// Whether this item has a crafting recipe or not
    /// </summary>
    public bool craftable;

    /// <summary>
    /// Array of ItemTypes that are required to craft this item. (Repeat items mean that multiple of a given ItemType are needed.)
    /// </summary>
    public ItemType[] ingredients;
}