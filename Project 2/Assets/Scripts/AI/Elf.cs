using UnityEngine;

/// <summary>
/// Represents an elf agent that moves around the map and completes task. It also includes visual
/// elements like an animator and the reference to the item being carried.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// 
/// DOCUMENTATION UNFINISHED
/// </summary>
public class Elf : Agent
{
    public enum ElfState
    {
        WaitingForTask,
        WalkingToTask,
        ProcessingTask,
    }

    public Animator animator;

    public Transform carryItemAnchor;

    public ItemType carryingItem;

    public ElfState state;

    public float maxSpeedWhenCarrying = 3;

    public float maxSpeedWhenWandering = 4;

    public float maxSpeedWhenNotCarrying = 5;

    private Task activeTask;

    private float stateStartTime;

    private GameObject carryItemObj;

    public void CarryItem(ItemType item)
    {
        carryingItem = item;

        if (item.carryPrefab != null)
        {
            carryItemObj = Instantiate(item.carryPrefab, carryItemAnchor, false);
        }
    }

    public void DropItem()
    {
        carryingItem = null;

        if (carryItemObj != null)
        {
            Destroy(carryItemObj);
        }
    }

    protected override void CalculateSteeringForces()
    {
        if (state == ElfState.WaitingForTask)
        {
            maxSpeed = maxSpeedWhenWandering;
        }
        else if (carryingItem != null)
        {
            maxSpeed = maxSpeedWhenCarrying;
        }
        else
        {
            maxSpeed = maxSpeedWhenNotCarrying;
        }

        switch (state)
        {
            case ElfState.WaitingForTask:
                Wander();
                AvoidAllObstacles(2);
                SeparatePredictive(AgentManager.Instance.agents, 5);

                TakeNewTask();
                break;
            case ElfState.WalkingToTask:
                Vector3 stationCenter = activeTask.GetStationCenter(activeTask.station);
                float stationRadius = activeTask.GetStationRadius(activeTask.station);
                float combinedRadius = stationRadius + physicsObject.radius;
                Vector3 seekDir = (stationCenter - physicsObject.Position).normalized;
                SeparatePredictive(AgentManager.Instance.agents, seekDir, 2);

                float disSqr = (physicsObject.Position - stationCenter).sqrMagnitude;
                if (disSqr > combinedRadius * combinedRadius)
                {
                    AvoidAllObstaclesAndSeek(stationCenter - seekDir * combinedRadius, 2);
                }
                else
                {
                    activeTask.StartTask();

                    state = ElfState.ProcessingTask;
                    stateStartTime = Time.time;
                }
                break;
            case ElfState.ProcessingTask:
                // Face the station's position if possible
                if (Vector3.SqrMagnitude(activeTask.station.transform.position - physicsObject.Position) > 0.1f)
                {
                    physicsObject.SetDirection((activeTask.station.transform.position - physicsObject.Position).normalized);
                }

                if (Time.time - stateStartTime >= activeTask.GetProcessingTime())
                {
                    activeTask.CompleteTask();
                    activeTask = null;

                    state = ElfState.WaitingForTask;
                    stateStartTime = Time.time;

                    TakeNewTask();
                }
                break;
        }

        StayInBounds();

        animator.SetFloat("WalkSpeed", physicsObject.Velocity.magnitude);
        animator.SetBool("Carrying", carryingItem != null);
    }

    private void TakeNewTask()
    {
        if (Taskmaster.Instance.TryTakeTask(this, out Task task))
        {
            activeTask = task;
            activeTask.InitializeTask();

            state = ElfState.WalkingToTask;
            stateStartTime = Time.time;
        }
    }
}