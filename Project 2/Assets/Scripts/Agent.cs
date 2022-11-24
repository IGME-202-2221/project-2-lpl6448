using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Parent class for all autonomous agents, which contains the ability to seek
/// or flee from different objects, depending on the subclass's implementation.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
[RequireComponent(typeof(PhysicsObject))]
public abstract class Agent : MonoBehaviour
{
    /// <summary>
    /// Reference to the PhysicsObject that this Agent is controlling
    /// </summary>
    public PhysicsObject physicsObject;

    /// <summary>
    /// Maximum speed that this Agent can move at
    /// </summary>
    public float maxSpeed = 5;

    /// <summary>
    /// Maximum total steering force that can be applied to this Agent
    /// </summary>
    public float maxForce = 5;

    /// <summary>
    /// Maximum angle (in degrees) that this Agent can wander from its current movement direction
    /// </summary>
    public float maxWanderAngle = 45;

    /// <summary>
    /// Maximum possible angle (in degrees) that the wander angle can change per second
    /// </summary>
    public float maxWanderChangePerSecond = 10;

    /// <summary>
    /// Radius that Agents will try to maintain between each other when applying the separate force
    /// </summary>
    public float personalSpace = 1;

    /// <summary>
    /// Maximum distance that an obstacle can be in front of this Agent before attempting to avoid the obstacle
    /// </summary>
    public float visionRange = 2;

    /// <summary>
    /// Accumulated steering forces to apply for this frame
    /// </summary>
    private Vector3 totalForce = Vector3.zero;

    /// <summary>
    /// Current angle that this Agent is wandering away from its current movement direction
    /// </summary>
    private float wanderAngle = 0;

    /// <summary>
    /// On awake, update the physicsObject to the one on this GameObject
    /// if none is already set
    /// </summary>
    private void Awake()
    {
        if (physicsObject == null)
        {
            physicsObject = GetComponent<PhysicsObject>();
        }
    }

    /// <summary>
    /// Every frame, calculate all steering forces, ensure that they do not exceed
    /// maxForce, and apply them to the physicsObject
    /// </summary>
    protected virtual void Update()
    {
        CalculateSteeringForces();

        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
        physicsObject.ApplyForce(totalForce);

        totalForce = Vector3.zero;
    }

    /// <summary>
    /// Calculates and applies all of the steering forces for this frame,
    /// using the Seek and Flee functions
    /// </summary>
    protected abstract void CalculateSteeringForces();


    /// <summary>
    /// Applies a seek steering force to the targetPos, with the specified weight
    /// </summary>
    /// <param name="targetPos">Vector3 that this Agent will seek towards</param>
    /// <param name="weight">Weight (multiplied into the seek steering force)</param>
    protected void Seek(Vector3 targetPos, float weight = 1)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = targetPos - physicsObject.Position;

        // Set desired velocity's magnitude
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate steering force
        Vector3 seekingForce = desiredVelocity - physicsObject.Velocity;

        // Apply steering force
        totalForce += seekingForce * weight;
    }

    /// <summary>
    /// Applies a flee steering force away from the targetPos, with the specified weight
    /// </summary>
    /// <param name="targetPos">Vector3 that this Agent will flee from</param>
    /// <param name="weight">Weight (multiplied into the flee steering force)</param>
    protected void Flee(Vector3 targetPos, float weight = 1)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = physicsObject.Position - targetPos;

        // Set desired velocity's magnitude
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate steering force
        Vector3 fleeingForce = desiredVelocity - physicsObject.Velocity;

        // Apply steering force
        totalForce += fleeingForce * weight;
    }

    /// <summary>
    /// Slightly varies the wanderAngle (depending on maxWanderAngle and maxWanderChangePerSecond)
    /// and applies a seek force in the wander direction
    /// </summary>
    /// <param name="weight">Weight (multiplied into the wander steering force)</param>
    protected void Wander(float weight = 1)
    {
        // Update the angle of current wander
        float maxWanderChange = maxWanderChangePerSecond * Time.deltaTime;
        wanderAngle += Random.Range(-maxWanderChange, maxWanderChange);
        wanderAngle = Mathf.Clamp(wanderAngle, -maxWanderAngle, maxWanderAngle);

        // Get a position that is in the current wander direction
        Vector3 wanderTarget = Quaternion.Euler(0, wanderAngle, 0) * physicsObject.Direction.normalized
            + physicsObject.Position;

        // Seek toward wander position
        Seek(wanderTarget, weight);
    }

    /// <summary>
    /// If this Agent is approaching the boundary of the screen, applies a seek force
    /// toward the middle of the screen. Specifically, if the Agent's future position (at t = 1)
    /// is outside of the boundaries or touching the boundaries (+- radius), the force is applied.
    /// </summary>
    /// <param name="weight">Weight (multiplied into the seek steering force to stay in bounds)</param>
    protected void StayInBounds(float weight = 1)
    {
        Vector3 futurePosition = GetFuturePosition();

        if (futurePosition.x < AgentManager.Instance.minPosition.x + physicsObject.radius
            || futurePosition.x > AgentManager.Instance.maxPosition.x - physicsObject.radius
            || futurePosition.z < AgentManager.Instance.minPosition.z + physicsObject.radius
            || futurePosition.z > AgentManager.Instance.maxPosition.z - physicsObject.radius)
        {
            Seek(Vector3.zero, weight);
        }
    }

    /// <summary>
    /// Computes where this Agent will be after timeAhead seconds, assuming its current
    /// velocity remains constant
    /// </summary>
    /// <param name="timeAhead">Time (in seconds) to multiply by the velocity to find the future position</param>
    /// <returns>Future position of this Agent after timeAhead seconds</returns>
    public Vector3 GetFuturePosition(float timeAhead = 1)
    {
        return physicsObject.Position + physicsObject.Velocity * timeAhead;
    }

    /// <summary>
    /// Applies a seek steering force toward either the future position of another agent
    /// or the current position of another agent, depending on distance
    /// </summary>
    /// <param name="other">Agent to pursue</param>
    /// <param name="timeAhead">Time (in seconds) used to calculate the other Agent's future position</param>
    /// <param name="weight">Weight (multiplied into the pursue steering force)</param>
    protected void Pursue(Agent other, float timeAhead = 1, float weight = 1)
    {
        Vector3 otherFuturePos = other.GetFuturePosition(timeAhead);
        float futurePosSqrDis = Vector3.SqrMagnitude(otherFuturePos - other.physicsObject.Position);
        float sqrDisToOther = Vector3.SqrMagnitude(physicsObject.Position - other.physicsObject.Position);

        if (futurePosSqrDis < sqrDisToOther)
        {
            Seek(otherFuturePos, weight);
        }
        else
        {
            Seek(other.physicsObject.Position, weight);
        }
    }

    /// <summary>
    /// Applies a flee steering force from either the future position of another agent
    /// or the current position of another agent, depending on distance
    /// </summary>
    /// <param name="other">Agent to flee from</param>
    /// <param name="timeAhead">Time (in seconds) used to calculate the other Agent's future position</param>
    /// <param name="weight">Weight (multiplied into the evade steering force)</param>
    protected void Evade(Agent other, float timeAhead = 1, float weight = 1)
    {
        Vector3 otherFuturePos = other.GetFuturePosition(timeAhead);
        float futurePosSqrDis = Vector3.SqrMagnitude(otherFuturePos - other.physicsObject.Position);
        float sqrDisToOther = Vector3.SqrMagnitude(physicsObject.Position - other.physicsObject.Position);

        if (futurePosSqrDis < sqrDisToOther)
        {
            Flee(otherFuturePos, weight);
        }
        else
        {
            Flee(other.physicsObject.Position, weight);
        };
    }

    /// <summary>
    /// Applies a separate steering force to avoid collisions with other Agents
    /// </summary>
    /// <typeparam name="T">Type of Agent in the agents list</typeparam>
    /// <param name="agents">List of Agents to avoid</param>
    /// <param name="weight">Weight (multiplied into the separate steering force)</param>
    protected void Separate<T>(List<T> agents, float weight) where T : Agent
    {
        float sqrPersonalSpace = personalSpace * personalSpace;

        foreach (T other in agents)
        {
            float sqrDis = Vector3.SqrMagnitude(other.physicsObject.Position - physicsObject.Position);

            // If other is this agent, skip
            if (sqrDis < float.Epsilon)
            {
                continue;
            }

            if (sqrDis < sqrPersonalSpace)
            {
                float disWeight = sqrPersonalSpace / (sqrDis + 0.1f);
                Flee(other.physicsObject.Position, disWeight * weight);
            }
        }
    }

    /// <summary>
    /// If this Agent is predicted to collide with an obstacle,
    /// applies a steering force to avoid colliding with the obstacle.
    /// </summary>
    /// <param name="obstacle">Obstacle to avoid if necessary</param>
    /// <param name="weight">Weight (multiplied into the obstacle avoidance steering force)</param>
    protected void AvoidObstacle(Obstacle obstacle, float weight = 1)
    {
        if (IsObstacleBlocking(obstacle, physicsObject.Direction, out Vector2 dis))
        {
            // Steer away from the obstacle
            Vector3 desiredVelocity = physicsObject.Right * Mathf.Sign(dis.x) * -maxSpeed; // Sign function used to avoid if statements
            float disWeight = visionRange / (dis.y + 0.1f);
            Vector3 steeringForce = (desiredVelocity - physicsObject.Velocity) * weight * disWeight;

            totalForce += steeringForce;
        }
    }

    protected bool IsObstacleBlocking(Obstacle obstacle, Vector3 dir, out Vector2 dis)
    {
        dis = Vector2.zero;
        Vector3 rightDir = new Vector3(dir.z, 0, -dir.x);

        // Check if the obstacle is behind this Agent
        Vector3 obstacleOffset = obstacle.Position - physicsObject.Position;
        float forwardDis = Vector3.Dot(obstacleOffset, dir);
        if (forwardDis < 0)
        {
            return false;
        }

        // Check if the obstacle is too far left/right
        float rightDis = Vector3.Dot(obstacleOffset, rightDir);
        float combinedRadius = obstacle.radius + physicsObject.radius;
        if (Mathf.Abs(rightDis) > combinedRadius)
        {
            return false;
        }

        // Check if the obstacle is too far forward
        if (forwardDis > visionRange)
        {
            return false;
        }

        dis = new Vector2(rightDis, forwardDis);

        return true;
    }

    protected void AvoidAllObstaclesAndSeek(Vector3 target, float weight = 1)
    {
        Vector3 targetDir = (target - physicsObject.Position).normalized;
        Vector3 rightDir = new Vector3(targetDir.z, 0, -targetDir.x);

        Vector3 sumDirs = Vector3.zero;
        float sumWeight = 0;
        foreach (Obstacle obstacle in ObstacleManager.Instance.obstacles)
        {
            float combinedRadius = obstacle.radius + physicsObject.radius;
            float sqrDis = (obstacle.Position - physicsObject.Position).sqrMagnitude;
            Vector3 obstacleDir = (obstacle.Position - physicsObject.Position).normalized;
            Vector3 obstacleRightDir = new Vector3(obstacleDir.z, 0, -obstacleDir.x);
            if (sqrDis > (combinedRadius - 0.0f) * (combinedRadius - 0.0f))
            {
                if (IsObstacleBlocking(obstacle, targetDir, out Vector2 dis))
                {
                    Vector3 circleCenter = (obstacle.Position + physicsObject.Position) / 2;
                    float circleRadius = Mathf.Sqrt(sqrDis) / 2;

                    // https://mathworld.wolfram.com/Circle-CircleIntersection.html
                    float disAlongLine = (circleRadius * circleRadius - combinedRadius * combinedRadius + circleRadius * circleRadius)
                        / circleRadius / 2;
                    float disPerpToLine = Mathf.Sqrt(circleRadius * circleRadius - disAlongLine * disAlongLine);
                    Vector3 intersectionPoint = circleCenter + obstacleDir * disAlongLine - obstacleRightDir * Mathf.Sign(dis.x) * disPerpToLine;

                    Vector3 seekPos = intersectionPoint;
                    Vector3 seekDir = (seekPos - physicsObject.Position).normalized;
                    if (seekDir.sqrMagnitude < 0.1f)
                    {
                        seekDir = targetDir;
                    }

                    float disWeight = visionRange * visionRange / (dis.y * dis.y + 0.1f);
                    sumDirs += seekDir * disWeight;
                    sumWeight += disWeight;
                }
            }
            else if (sqrDis < (combinedRadius - 0.0f) * (combinedRadius - 0.0f))
            {
                //float circleRadius = Mathf.Sqrt(sqrDis);
                //float rightDis = Vector3.Dot(obstacle.Position - physicsObject.Position, physicsObject.Right);

                //// https://mathworld.wolfram.com/Circle-CircleIntersection.html
                //float disAlongLine = (circleRadius * circleRadius - combinedRadius * combinedRadius + circleRadius * circleRadius)
                //    / circleRadius / 2;
                //float disPerpToLine = Mathf.Sqrt(circleRadius * circleRadius - disAlongLine * disAlongLine);
                //Vector3 intersectionPoint = obstacle.Position + obstacleDir * disAlongLine - obstacleRightDir * Mathf.Sign(rightDis) * disPerpToLine;

                //Vector3 seekPos = intersectionPoint;
                //Vector3 seekDir = (seekPos - physicsObject.Position).normalized;

                //Seek(physicsObject.Position + seekDir.normalized, weight);
                Seek(physicsObject.Position + (physicsObject.Position - obstacle.Position).normalized, weight);
                return;
            }
        }

        Vector3 seekAvoidDir = sumDirs / sumWeight;
        if (sumWeight == 0 || Vector3.Dot(seekAvoidDir, targetDir) < 0)
        {
            Seek(target, weight);
        }
        else
        {
            Seek(physicsObject.Position + seekAvoidDir, weight);
        }
    }

    /// <summary>
    /// Applies steering forces to avoid any obstacles that this Agent is predicted to collide with.
    /// All of the Obstacles in ObstacleManager.Instance.obstacles are checked
    /// </summary>
    /// <param name="weight">Weight (multiplied into the obstacle avoidance steering forces)</param>
    protected void AvoidAllObstacles(float weight = 1)
    {
        foreach (Obstacle obstacle in ObstacleManager.Instance.obstacles)
        {
            AvoidObstacle(obstacle, weight);
        }
    }

    /// <summary>
    /// When selected, draw gizmos to show the PhysicsObject's radius (red)
    /// and the personalSpace radius (green)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, physicsObject.radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalSpace);
    }
}