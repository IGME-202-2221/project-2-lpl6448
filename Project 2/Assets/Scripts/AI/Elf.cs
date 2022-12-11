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

    public ElfCanvas elfCanvas;

    public Transform carryItemAnchor;

    public ItemType carryingItem;

    public ElfState state;

    public float maxSpeedWhenCarrying = 3;

    public float maxSpeedWhenWandering = 4;

    public float maxSpeedWhenNotCarrying = 5;

    private Task activeTask;

    private float stateStartTime;

    private ItemObject carryItemObj;

    public void CarryItem(ItemType item)
    {
        carryingItem = item;

        if (item.objectPrefab != null)
        {
            carryItemObj = Instantiate(item.objectPrefab, carryItemAnchor, false);
            carryItemObj.PositionObjectForCarrying();
        }
    }

    public void DropItem()
    {
        carryingItem = null;

        if (carryItemObj != null)
        {
            Destroy(carryItemObj.gameObject);
            carryItemObj = null;
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

        // Make a question mark appear above the elf's head if he does not have a task
        elfCanvas.canvas.enabled = state == ElfState.WaitingForTask;

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
                SeparatePredictive(AgentManager.Instance.agents, seekDir, 4);

                float disSqr = (physicsObject.Position - stationCenter).sqrMagnitude;
                if (disSqr > combinedRadius * combinedRadius)
                {
                    AvoidAllObstaclesAndSeek(stationCenter - seekDir * combinedRadius, 1);
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

                if (activeTask is UseTask && !string.IsNullOrEmpty(activeTask.station.UsingAnimation))
                {
                    animator.SetBool(activeTask.station.UsingAnimation, true);
                }

                if (Time.time - stateStartTime >= activeTask.GetProcessingTime())
                {
                    if (activeTask is UseTask && !string.IsNullOrEmpty(activeTask.station.UsingAnimation))
                    {
                        animator.SetBool(activeTask.station.UsingAnimation, false);
                    }

                    activeTask.CompleteTask();
                    activeTask = null;

                    state = ElfState.WaitingForTask;
                    stateStartTime = Time.time;

                    TakeNewTask();
                }
                break;
        }

        // Stay in bounds for any state
        StayInBounds(-WorldManager.Instance.elfWorldExtents, WorldManager.Instance.elfWorldExtents);

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