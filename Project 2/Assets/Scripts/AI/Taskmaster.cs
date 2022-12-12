using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager class that contains information about current items being build and current tasks
/// and decides which Tasks each Elf will take. The items being crafted are decided by the user.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class Taskmaster : MonoBehaviour
{
    /// <summary>
    /// Singleton Taskmaster in the scene
    /// </summary>
    public static Taskmaster Instance;

    /// <summary>
    /// List of root-level ItemTypes that must be crafted.
    /// </summary>
    public List<ItemType> requestedItems = new List<ItemType>();

    /// <summary>
    /// List of all ItemTypes that must be crafted in order to complete the requestedItems, including the
    /// requestedItems list.
    /// </summary>
    public List<ItemType> itemsToCraft = new List<ItemType>();

    /// <summary>
    /// List of Tasks that have not been assigned to an Elf yet
    /// </summary>
    private List<Task> tasks = new List<Task>();

    /// <summary>
    /// Adds the task to the list of tasks that Elves can take and refreshes the frontend to reflect the new Task
    /// </summary>
    /// <param name="task">Task to add to the list of tasks that Elves can take</param>
    public void AddTask(Task task)
    {
        tasks.Add(task);
        UIManager.Instance.RefreshCanBuild();
    }

    /// <summary>
    /// Tries to take any Task from the list of Tasks for the given Elf, assigning the Task
    /// to the Elf if successful. The closest valid Task to the given Elf (using the task's Priorities as a tiebreaker)
    /// is chosen.
    /// </summary>
    /// <param name="elf">Elf that a Task will be assigned to if possible</param>
    /// <param name="task">Task that is assigned to the Elf, or null if no Task is found</param>
    /// <returns>Whether a Task was assigned to the Elf</returns>
    public bool TryTakeTask(Elf elf, out Task task)
    {
        Task closestTask = null;
        float closestDisSqr = float.MaxValue;
        foreach (Task possibleTask in tasks)
        {
            if (possibleTask.CanTake(elf))
            {
                Station source = possibleTask.GetStationForElf(elf);
                float disSqr = (possibleTask.GetStationCenter(source) - elf.physicsObject.Position).sqrMagnitude;
                if (disSqr < closestDisSqr || Mathf.Approximately(disSqr, closestDisSqr) && possibleTask.Priority > closestTask.Priority)
                {
                    closestTask = possibleTask;
                    closestDisSqr = disSqr;
                }
            }
        }

        task = closestTask;
        if (task != null)
        {
            tasks.Remove(task);
            task.AttemptToAssignTask(elf);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Registers that the item will be crafted soon, removing it from the itemsToCraft list and
    /// telling the station that the item will be crafted there
    /// </summary>
    /// <param name="item">ItemType to begin crafting at the station</param>
    /// <param name="station">Station that the item will be crafted at</param>
    public void BeginBuildingItem(ItemType item, Station station)
    {
        itemsToCraft.Remove(item);
        station.AcceptUserItem(item);
    }

    /// <summary>
    /// Registers that the item has been fully crafted, refreshing the frontend to reflect this
    /// and creating a GatherTask to deliver it to the FinishStation if this item is in requestedItems
    /// </summary>
    /// <param name="item">ItemType that has just been crafted</param>
    public void FinishBuildingItem(ItemType item)
    {
        UIManager.Instance.RefreshCanBuild();

        if (requestedItems.Contains(item))
        {
            GatherTask task = new GatherTask(item);
            AddTask(task);
        }
    }

    /// <summary>
    /// Determines whether the given item is in the itemsToCraft list
    /// </summary>
    /// <param name="item">ItemType that may be crafted</param>
    /// <returns>Whether the item is in the itemsToCraft list</returns>
    public bool CanBuildItem(ItemType item)
    {
        return itemsToCraft.Contains(item);
    }

    /// <summary>
    /// Initializes the singleton, creates the itemsToCraft list from the requestedItems,
    /// and populates the frontend CraftingPanel with the requestedItems
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (ItemType item in requestedItems)
        {
            InitItem(item);
        }

        UIManager.Instance.craftingPanel.Populate(requestedItems);
    }

    /// <summary>
    /// Recursively initializes the item and all of its ingredients, adding them to the itemsToCraft list
    /// </summary>
    /// <param name="item">ItemType to initialize</param>
    private void InitItem(ItemType item)
    {
        if (item.craftable)
        {
            itemsToCraft.Add(item);
            foreach (ItemType ingredient in item.ingredients)
            {
                InitItem(ingredient);
            }
        }
    }
}