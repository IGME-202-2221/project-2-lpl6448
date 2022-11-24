﻿using UnityEngine;
using System.Collections.Generic;

public class Taskmaster : MonoBehaviour
{
    public static Taskmaster Instance;

    public List<ItemType> initItems = new List<ItemType>();

    public List<ItemType> itemsBuilding = new List<ItemType>();

    private List<Task> tasks = new List<Task>();

    private Queue<ItemType> itemsToBuild = new Queue<ItemType>();

    public void AddTask(Task task)
    {
        tasks.Add(task);
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
        if (itemsToBuild.Count > 0)
        {
            ItemType newItem = itemsToBuild.Dequeue();
            AddItemToCurrentlyBuilding(newItem);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (ItemType item in initItems)
        {
            InitItem(item);
        }

        //for (int i = 0; i < WorldManager.Instance.stations.Count; i++)
        while (itemsToBuild.Count > 0)
        {
            //Station station = WorldManager.Instance.stations[i];
            //if (station is ToyStation)
            {
                ItemType newItem = itemsToBuild.Dequeue();
                AddItemToCurrentlyBuilding(newItem);

                //if (initItems.Contains(newItem))
                //{
                //    i--;
                //}
            }
        }
    }

    private void InitItem(ItemType item)
    {
        if (item.craftable)
        {
            itemsToBuild.Enqueue(item);
            foreach (ItemType ingredient in item.ingredients)
            {
                InitItem(ingredient);
            }
        }
    }

    private void AddItemToCurrentlyBuilding(ItemType item)
    {
        if (item.craftable)
        {
            itemsBuilding.Add(item);
            foreach (ItemType ingredient in item.ingredients)
            {
                if (!ingredient.craftable)
                {
                    AddTask(new GatherTask(ingredient));
                }
            }
        }
    }
}