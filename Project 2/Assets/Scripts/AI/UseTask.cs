using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Task where a Station must be used by an Elf, currently used for
/// building items at a ToyStation.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class UseTask : Task
{
    /// <summary>
    /// Creates a new UseTask, where an action will be completed at the given station
    /// </summary>
    /// <param name="station">Station that this Task will be performed at</param>
    public UseTask(Station station)
    {
        SetStation(station);
    }

    /// <summary>
    /// If there are multiple tasks at the same Station, the Priority is used to determine
    /// which Task will be taken by that Elf
    /// </summary>
    public override float Priority => 2;

    /// <summary>
    /// Gets the center of the station's action circle, which an Elf must be touching in order
    /// to complete the Task. The UseTask's station center is the station's elfUseCenter.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Station's elfUseCenter</returns>
    public override Vector3 GetStationCenter(Station station) => station.ElfUseCenter;

    /// <summary>
    /// Gets the radius of the station's action circle, which an Elf must be touching in order
    /// to complete the Task. The UseTask's station radius is the station's elfUseRadius.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Station's elfUseRadius</returns>
    public override float GetStationRadius(Station station) => station.elfUseRadius;

    /// <summary>
    /// Gets the number of seconds for an Elf to complete this Task, after approaching the station
    /// </summary>
    /// <returns>Number of seconds for an Elf to complete this Task</returns>
    public override float GetProcessingTime() => 6;

    /// <summary>
    /// Determines whether this Task can be taken by a particular Elf. An Elf can take a UseTask
    /// if the station can currently be used.
    /// </summary>
    /// <param name="elf">Elf that is considering taking this Task</param>
    /// <returns>Whether this Task can be taken by the elf</returns>
    public override bool CanTake(Elf elf)
    {
        return station.CanUse();
    }

    /// <summary>
    /// Called when this Task is assigned to an Elf, telling the station that it will be used soon
    /// </summary>
    public override void InitializeTask()
    {
        station.PrepareToUse();
    }

    /// <summary>
    /// Called when the assignee Elf approaches the Station and starts processing this Task, telling the station to begin being used
    /// </summary>
    public override void StartTask()
    {
        station.BeginUse();
    }

    /// <summary>
    /// Called after the processing time has been completed by the assignee Elf and this Task has been completed, telling the station to end being used
    /// </summary>
    public override void CompleteTask()
    {
        station.EndUse();
    }
}