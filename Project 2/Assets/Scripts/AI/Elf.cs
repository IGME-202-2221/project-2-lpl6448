using UnityEngine;

/// <summary>
/// Represents an elf agent that moves around the map and completes task. It also includes visual
/// elements like an animator and the reference to the item being carried.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class Elf : Agent
{
    /// <summary>
    /// Contains the three main states that Elves can be in (not including Tasks)
    /// WaitingForTask: The Elf has no Task currently but tries to claim a new one every frame.
    /// WalkingToTask: The Elf is walking to the Station of its active Task.
    /// ProcessingTask: The Elf is at the Station of its active Task, completing the Task.
    /// </summary>
    public enum ElfState
    {
        WaitingForTask,
        WalkingToTask,
        ProcessingTask,
    }

    /// <summary>
    /// Reference to the Animator for this Elf, used to control its model and animation
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Reference to the ElfCanvas for this Elf, used to make a question mark appear above Elves without Tasks
    /// </summary>
    public ElfCanvas elfCanvas;

    /// <summary>
    /// Anchor/parent Transform of the item that is being carried by this Elf
    /// </summary>
    public Transform carryItemAnchor;

    /// <summary>
    /// ItemType that this Elf is currently carrying (or null if no item is being carried)
    /// </summary>
    public ItemType carryingItem;

    /// <summary>
    /// This Elf's current ElfState
    /// </summary>
    public ElfState state;

    /// <summary>
    /// Max speed of the Agent when an item is being carried
    /// </summary>
    public float maxSpeedWhenCarrying = 3;

    /// <summary>
    /// Max speed of the Agent when this Elf is wandering
    /// </summary>
    public float maxSpeedWhenWandering = 4;

    /// <summary>
    /// Max speed of the Agent when an item is not being carried (when this Elf is walking to collect an item)
    /// </summary>
    public float maxSpeedWhenNotCarrying = 5;

    /// <summary>
    /// Current Task that this Elf is trying to complete
    /// </summary>
    private Task activeTask;

    /// <summary>
    /// In-game time when this Elf most recently changed its state
    /// </summary>
    private float stateStartTime;

    /// <summary>
    /// Current ItemObject being carried by the elf (or null if no item is being carried)
    /// </summary>
    private ItemObject carryItemObj;

    /// <summary>
    /// Begins to visibly carry the given item by instantiating the item's ItemObject
    /// </summary>
    /// <param name="item">ItemType to carry</param>
    public void CarryItem(ItemType item)
    {
        carryingItem = item;

        if (item.objectPrefab != null)
        {
            carryItemObj = Instantiate(item.objectPrefab, carryItemAnchor, false);
            carryItemObj.PositionObjectForCarrying();
        }
    }

    /// <summary>
    /// Drops the current carrying item, destroying the ItemObject
    /// </summary>
    public void DropItem()
    {
        carryingItem = null;

        if (carryItemObj != null)
        {
            Destroy(carryItemObj.gameObject);
            carryItemObj = null;
        }
    }

    /// <summary>
    /// Calculates and applies all of the steering forces for this frame, based on the Elf's state,
    /// and does additional per-frame logic to update the animator and process Tasks
    /// </summary>
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

    /// <summary>
    /// Attempts to take a new Task. If there are no Tasks available, this function is called
    /// every frame until a Task becomes available.
    /// </summary>
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