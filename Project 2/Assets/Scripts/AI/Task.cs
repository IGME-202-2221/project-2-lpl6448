using UnityEngine;

/// <summary>
/// Represents an instruction for an Elf, requiring that the Elf arrive at a particular Station
/// and do a particular action at that station.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public abstract class Task
{
    /// <summary>
    /// Station that the action will be performed at
    /// </summary>
    public Station station { get; private set; }

    /// <summary>
    /// Elf that will complete this Task (null if no Elf has taken it yet)
    /// </summary>
    public Elf assignee { get; private set; }

    /// <summary>
    /// If there are multiple tasks at the same Station, the Priority is used to determine
    /// which Task will be taken by that Elf
    /// </summary>
    public abstract float Priority { get; }

    /// <summary>
    /// Manually sets the station to perform this Task at
    /// </summary>
    /// <param name="station">Station perform this Task at</param>
    public void SetStation(Station station)
    {
        this.station = station;
    }

    /// <summary>
    /// Finds the Station that this Task will be performed at. This function can
    /// contain additional logic that will choose what the station property will be set to.
    /// </summary>
    /// <param name="elf">Elf that is considering taking or has just taken this Task</param>
    /// <returns>Station that this Task will be performed at</returns>
    public virtual Station GetStationForElf(Elf elf) => station;

    /// <summary>
    /// Gets the center of the station's action circle, which an Elf must be touching in order
    /// to complete the Task.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Center of the station's action circle</returns>
    public abstract Vector3 GetStationCenter(Station station);

    /// <summary>
    /// Gets the radius of the station's action circle, which an Elf must be touching in order
    /// to complete the Task.
    /// </summary>
    /// <param name="station">Station that the Task will be performed at</param>
    /// <returns>Radius of the station's action circle</returns>
    public abstract float GetStationRadius(Station station);

    /// <summary>
    /// Gets the number of seconds for an Elf to complete this Task, after approaching the station
    /// </summary>
    /// <returns>Number of seconds for an Elf to complete this Task</returns>
    public abstract float GetProcessingTime();

    /// <summary>
    /// Tries to assign this Task to the given Elf, setting the station if the Task is successfully assigned
    /// </summary>
    /// <param name="elf">Elf to attempt to assign this Task to</param>
    /// <returns>Whether this Task was successfully assigned or not</returns>
    public bool AttemptToAssignTask(Elf elf)
    {
        if (CanTake(elf))
        {
            assignee = elf;
            if (station == null)
            {
                station = GetStationForElf(elf);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether this Task can be taken by a particular Elf
    /// </summary>
    /// <param name="elf">Elf that is considering taking this Task</param>
    /// <returns>Whether this Task can be taken by the elf</returns>
    public abstract bool CanTake(Elf elf);

    /// <summary>
    /// Called when this Task is assigned to an Elf
    /// </summary>
    public abstract void InitializeTask();

    /// <summary>
    /// Called when the assignee Elf approaches the Station and starts processing this Task
    /// </summary>
    public abstract void StartTask();

    /// <summary>
    /// Called after the processing time has been completed by the assignee Elf and this Task has been completed
    /// </summary>
    public abstract void CompleteTask();
}