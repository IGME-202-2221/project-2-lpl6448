using UnityEngine;

/// <summary>
/// Represents a snowball that will fly through the air and create an Obstacle when it lands.
/// (This class does not inherit from PhysicsObject because it needs 3D physics.)
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class Snowball : MonoBehaviour
{
    /// <summary>
    /// Obstacle prefab to instantiate at this Snowball's position when it lands
    /// </summary>
    public Obstacle landObstacle;

    /// <summary>
    /// Acceleration due to gravity for this Snowball, applied every frame
    /// </summary>
    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    
    /// <summary>
    /// Current velocity of this Snowball
    /// </summary>
    public Vector3 velocity = Vector3.zero;

    /// <summary>
    /// Every frame, updates the velocity based on gravity and updates the position based on velocity.
    /// If the position's y-value is <= 0, the Snowball lands.
    /// </summary>
    private void Update()
    {
        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        if (transform.position.y <= 0)
        {
            Land();
        }
    }

    /// <summary>
    /// Called when the Snowball lands to destroy the Snowball and instantiate landObstacle at the landing position
    /// </summary>
    private void Land()
    {
        Instantiate(landObstacle, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        Destroy(gameObject);
    }
}