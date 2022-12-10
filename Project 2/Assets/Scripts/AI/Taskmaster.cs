using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager class that contains information about current items being build and current tasks
/// and decides which Tasks each Elf will take. The algorithm that decides which items will be
/// built is currently simple and prone to deadlock, but it may be improved by allowing user
/// interaction in the future.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class Taskmaster : MonoBehaviour
{
    public static Taskmaster Instance;

    public List<ItemType> requestedItems = new List<ItemType>();

    public List<ItemType> itemsToCraft = new List<ItemType>();

    private List<Task> tasks = new List<Task>();

    public void AddTask(Task task)
    {
        tasks.Add(task);
        UIManager.Instance.RefreshCanBuild();
    }

    public bool TryTakeTask(Elf elf, out Task task)
    {
        Task closestTask = null;
        float closestDisSqr = float.MaxValue;
        foreach (Task possibleTask in tasks)
        {
            if (possibleTask.CanTake(elf))
            {
                Station source = possibleTask.GetSourceForElf(elf);
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

    public void FinishBuildingItem(ItemType item)
    {
        //if (itemsToBuild.Count > 0)
        //{
        //    ItemType newItem = itemsToBuild.Dequeue();
        //    AddItemToCurrentlyBuilding(newItem);
        //}
        UIManager.Instance.RefreshCanBuild();

        if (requestedItems.Contains(item))
        {
            GatherTask task = new GatherTask(item);
            AddTask(task);
        }
    }

    public void BeginBuildingItem(ItemType item, Station station)
    {
        itemsToCraft.Remove(item);
        station.AcceptUserItem(item);
    }

    public bool CanBuildItem(ItemType item)
    {
        // When all of the currently building items are finished, another of this item still must be needed
        if (!itemsToCraft.Contains(item))
        {
            return false;
        }

        // At least one station must be able to accept this item
        //foreach (Station station in WorldManager.Instance.stations)
        //{
        //    if (station.CanAcceptUserItem(item))
        //    {
        //        return true;
        //    }
        //}
        //return false;

        return true;
    }

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