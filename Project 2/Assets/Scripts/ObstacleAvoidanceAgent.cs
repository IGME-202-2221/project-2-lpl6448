using UnityEngine;

/// <summary>
/// Represents an Agent that will wander, stay in bounds, separate itself
/// from other Agents, and avoid any nearby obstacles.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class ObstacleAvoidanceAgent : Agent
{
    /// <summary>
    /// Called every frame to steer this Agent, making it wander,
    /// stay in bounds, separate itself from other Agents, and
    /// avoid any nearby obstacles
    /// </summary>
    protected override void CalculateSteeringForces()
    {
        Wander();
        StayInBounds(3);
        Separate(AgentManager.Instance.agents, 1);

        AvoidAllObstacles(3);
    }
}