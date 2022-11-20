using UnityEngine;

/// <summary>
/// Represents an obstacle with a radius that Agents should attempt to avoid
/// if using the obstacle avoidance steering force.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class Obstacle : MonoBehaviour
{
    /// <summary>
    /// Circle radius of this obstacle (region to be avoided)
    /// </summary>
    public float radius = 1;

    /// <summary>
    /// World position of this Obstacle
    /// </summary>
    public Vector3 Position => transform.position;

    /// <summary>
    /// When this Obstacle is selected, draws a red wire sphere around it
    /// to represent its radius
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Position, radius);
    }
}