using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an object that is controlled by a physics simulation, involving
/// forces, acceleration, velocity, mass, etc.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class PhysicsObject : MonoBehaviour
{
    /// <summary>
    /// The mass of this object, used to calculate the acceleration from a force (F = ma)
    /// </summary>
    public float mass = 1;

    /// <summary>
    /// Whether this object should bounce off of the "walls" of the map or not
    /// </summary>
    public bool bounceOffWalls = false;

    /// <summary>
    /// Whether this object is affected by gravity or not
    /// </summary>
    public bool useGravity = false;

    /// <summary>
    /// Acceleration (in the downward direction) due to gravity
    /// </summary>
    public float gravityStrength = 1;

    /// <summary>
    /// Whether this object is affected by friction or not
    /// </summary>
    public bool useFriction = false;

    /// <summary>
    /// Coefficient of friction (magnitude of force in the reverse direction of velocity)
    /// </summary>
    public float frictionCoefficient = 0.2f;

    /// <summary>
    /// This object's radius (used for circle collision checking)
    /// </summary>
    public float radius = 1;

    /// <summary>
    /// Current velocity of this object
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// Current acceleration of this object
    /// </summary>
    private Vector3 acceleration;

    /// <summary>
    /// Normalized direction of the velocity
    /// </summary>
    private Vector3 direction;

    /// <summary>
    /// Current velocity of this object
    /// </summary>
    public Vector3 Velocity => velocity;

    /// <summary>
    /// Normalized direction of the velocity
    /// </summary>
    public Vector3 Direction => direction;

    /// <summary>
    /// Normalized right direction of this object
    /// </summary>
    public Vector3 Right => transform.right;

    /// <summary>
    /// Current position of this object in the world
    /// </summary>
    public Vector3 Position => transform.position;

    /// <summary>
    /// On game start, initialize the cameraSize to the x and y extents of the Camera view
    /// </summary>
    private void Start()
    {
        direction = Random.insideUnitCircle.normalized;
    }

    /// <summary>
    /// Every frame, apply forces (gravity and friction) for this object and
    /// update the object's velocity, position, and direction. Also, bounce off the walls
    /// of the screen if necessary.
    /// </summary>
    private void Update()
    {
        // Apply gravity and friction
        if (useGravity)
        {
            ApplyGravity(Vector3.down * gravityStrength);
        }
        if (useFriction)
        {
            ApplyFriction(frictionCoefficient);
        }


        // Calculate a new velocity based on acceleration
        velocity += acceleration * Time.deltaTime;

        // Calculate the new position based on velocity
        transform.position += velocity * Time.deltaTime;

        // Store the direction of motion and rotate the object to face it
        if (velocity.sqrMagnitude > Mathf.Epsilon)
        {
            direction = velocity.normalized;
        }
        transform.rotation = Quaternion.LookRotation(Vector3.back, direction);

        // Zero out acceleration for the next frame
        acceleration = Vector3.zero;

        // Bounce if necessary
        if (bounceOffWalls)
        {
            Bounce();
        }
    }

    /// <summary>
    /// Applies a force to this object following Newton's second law of motion
    /// </summary>
    /// <param name="force">Force vector acting on the object (affecting acceleration by force / mass)</param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// Applies a friction force to this object (in the reverse direction of velocity)
    /// </summary>
    /// <param name="coefficient">Coefficient of friction between this object and the contact surface</param>
    private void ApplyFriction(float coefficient)
    {
        ApplyForce(-velocity.normalized * coefficient);
    }

    /// <summary>
    /// Applies a gravity force to this object (modifying the acceleration)
    /// </summary>
    /// <param name="gravity">Gravity acceleration vector</param>
    private void ApplyGravity(Vector3 gravity)
    {
        acceleration += gravity;
    }

    /// <summary>
    /// Checks this object against all four edges of the screen and changes the velocity if it
    /// is outside the bounds and continuing to move outside of bounds.
    /// This function is implemented as it was in this week's demo and therefore checks if the
    /// center of the object is outside of the screen.
    /// </summary>
    private void Bounce()
    {
        // If offscreen and continuing to move offscreen, change the velocity (to bounce)
        if (transform.position.x > AgentManager.Instance.maxPosition.x && velocity.x > 0)
        {
            velocity.x *= -1;
        }
        if (transform.position.x < AgentManager.Instance.minPosition.x && velocity.x < 0)
        {
            velocity.x *= -1;
        }
        if (transform.position.y > AgentManager.Instance.maxPosition.y && velocity.y > 0)
        {
            velocity.y *= -1;
        }
        if (transform.position.y < AgentManager.Instance.minPosition.y && velocity.y < 0)
        {
            velocity.y *= -1;
        }
    }
}
