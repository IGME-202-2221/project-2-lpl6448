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
        Vector3 wanderTarget = Quaternion.Euler(0, 0, wanderAngle) * physicsObject.Direction.normalized
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
            || futurePosition.y < AgentManager.Instance.minPosition.y + physicsObject.radius
            || futurePosition.y > AgentManager.Instance.maxPosition.y - physicsObject.radius)
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