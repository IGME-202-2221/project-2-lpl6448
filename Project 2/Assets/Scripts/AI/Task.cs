using UnityEngine;

/// <summary>
/// Represents an instruction for an Elf, requiring that the Elf arrive at a particular Station
/// and do a particular action at that station.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public abstract class Task
{
    public Station station { get; private set; }

    public Elf assignee { get; private set; }

    public abstract float Priority { get; }

    public void SetStation(Station station)
    {
        this.station = station;
    }

    public virtual Station GetSourceForElf(Elf elf) { return station; }

    public abstract Vector3 GetStationCenter(Station station);

    public abstract float GetStationRadius(Station station);

    public abstract float GetProcessingTime();

    public bool AttemptToAssignTask(Elf elf)
    {
        if (CanTake(elf))
        {
            assignee = elf;
            if (station == null)
            {
                station = GetSourceForElf(elf);
            }
            return true;
        }
        return false;
    }

    public abstract bool CanTake(Elf elf);

    public abstract void InitializeTask();

    public abstract void StartTask();

    public abstract void CompleteTask();
}