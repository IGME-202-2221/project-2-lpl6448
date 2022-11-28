using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a Task where a Station must be used by an Elf.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class UseTask : Task
{
    public UseTask(Station station)
    {
        SetStation(station);
    }

    public override float Priority => 2;

    public override Vector3 GetStationCenter(Station station) => station.ElfUseCenter;

    public override float GetStationRadius(Station station) => station.elfUseRadius;

    public override float GetProcessingTime() => 6;

    public override bool CanTake(Elf elf)
    {
        return station.CanUse();
    }

    public override void InitializeTask()
    {
        station.PrepareToUse();
    }

    public override void StartTask()
    {
        station.BeginUse();
    }

    public override void CompleteTask()
    {
        station.EndUse();
    }
}