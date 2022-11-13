using UnityEngine;

/// <summary>
/// Represents an Agent that will wander, stay in bounds, and separate itself
/// from other Agents.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class SeparationAgent : Agent
{
    /// <summary>
    /// Called every frame to steer this Agent, making it wander,
    /// stay in bounds, and separate itself from other Agents
    /// </summary>
    protected override void CalculateSteeringForces()
    {
        Wander();
        StayInBounds(3);
        Separate(AgentManager.Instance.agents, 1);
    }
}