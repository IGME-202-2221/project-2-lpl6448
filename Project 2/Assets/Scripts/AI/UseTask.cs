using UnityEngine;
using System.Collections.Generic;

public class UseTask : Task
{
    public UseTask(Station source)
    {
        SetSource(source);
    }

    public override float Priority => 2;

    public override Vector3 GetStationCenter(Station station) => station.ElfUseCenter;

    public override float GetStationRadius(Station station) => station.elfUseRadius;

    public override float GetProcessingTime() => 6;

    public override bool CanTake(Elf elf)
    {
        return source.CanUse();
    }

    public override void InitializeTask()
    {
        source.PrepareToUse();
    }

    public override void StartTask()
    {
        source.BeginUse();
    }

    public override void CompleteTask()
    {
        source.EndUse();
    }
}