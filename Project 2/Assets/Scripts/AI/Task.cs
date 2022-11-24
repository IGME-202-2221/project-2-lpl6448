using UnityEngine;

public abstract class Task
{
    public Station source { get; private set; }

    public Elf assignee { get; private set; }

    public abstract float Priority { get; }

    public void SetSource(Station source)
    {
        this.source = source;
    }

    public virtual Station GetSourceForElf(Elf elf) { return source; }

    public abstract Vector3 GetStationCenter(Station station);

    public abstract float GetStationRadius(Station station);

    public abstract float GetProcessingTime();

    public bool AttemptToAssignTask(Elf elf)
    {
        if (CanTake(elf))
        {
            assignee = elf;
            if (source == null)
            {
                source = GetSourceForElf(elf);
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