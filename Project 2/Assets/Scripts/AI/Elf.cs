using UnityEngine;

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
        if (carryingItem != null)
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
                // Wander
                Separate(AgentManager.Instance.agents, 3);

                TakeNewTask();
                break;
            case ElfState.WalkingToTask:
                Vector3 stationCenter = activeTask.GetStationCenter(activeTask.source);
                float stationRadius = activeTask.GetStationRadius(activeTask.source);
                AvoidAllObstaclesAndSeek(stationCenter);
                Separate(AgentManager.Instance.agents, 3);

                float combinedRadius = stationRadius + physicsObject.radius;
                if ((physicsObject.Position - stationCenter).sqrMagnitude <= combinedRadius * combinedRadius)
                {
                    activeTask.StartTask();

                    state = ElfState.ProcessingTask;
                    stateStartTime = Time.time;
                }
                break;
            case ElfState.ProcessingTask:
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

        //StayInBounds();

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (state == ElfState.WalkingToTask)
        {
            Vector3 target = activeTask.GetStationCenter(activeTask.source);
            Vector3 targetDir = (target - physicsObject.Position).normalized;
            Vector3 rightDir = new Vector3(targetDir.z, 0, -targetDir.x);

            foreach (Obstacle obstacle in ObstacleManager.Instance.obstacles)
            {
                float combinedRadius = obstacle.radius + physicsObject.radius;
                float sqrDis = (obstacle.Position - physicsObject.Position).sqrMagnitude;
                if (sqrDis > (combinedRadius - 0.0f) * (combinedRadius - 0.0f))
                {
                    Vector3 obstacleDir = (obstacle.Position - physicsObject.Position).normalized;
                    Vector3 obstacleRightDir = new Vector3(obstacleDir.z, 0, -obstacleDir.x);
                    if (IsObstacleBlocking(obstacle, targetDir, out Vector2 dis))
                    {
                        Vector3 circleCenter = (obstacle.Position + physicsObject.Position) / 2;
                        float circleRadius = Mathf.Sqrt(sqrDis) / 2;

                        // https://mathworld.wolfram.com/Circle-CircleIntersection.html
                        float disAlongLine = (circleRadius * circleRadius - combinedRadius * combinedRadius + circleRadius * circleRadius)
                            / circleRadius / 2;
                        float disPerpToLine = Mathf.Sqrt(circleRadius * circleRadius - disAlongLine * disAlongLine);
                        Vector3 intersectionPoint = circleCenter + obstacleDir * disAlongLine - obstacleRightDir * Mathf.Sign(dis.x) * disPerpToLine;

                        Gizmos.DrawWireSphere(circleCenter, circleRadius);
                        Gizmos.DrawWireSphere(physicsObject.Position, physicsObject.radius);
                        Gizmos.DrawWireSphere(obstacle.Position, obstacle.radius);
                        Gizmos.DrawSphere(intersectionPoint, 0.2f);
                        Gizmos.DrawLine(physicsObject.Position, intersectionPoint);
                    }
                }
            }
        }
    }
}