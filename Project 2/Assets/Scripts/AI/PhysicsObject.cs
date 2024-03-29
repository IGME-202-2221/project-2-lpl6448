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
    /// Linear interpolation factor used to smooth this object's rotation
    /// </summary>
    public float rotationLerp = 0.99f;

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
    /// Manually sets this object's direction (which is still subject to change
    /// next frame if this object is moving)
    /// </summary>
    /// <param name="direction">New Vector3 direction vector of this object</param>
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    /// <summary>
    /// On game start, initialize the cameraSize to the x and y extents of the Camera view
    /// </summary>
    private void Start()
    {
        float rad = Random.value * Mathf.PI * 2;
        direction = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
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
        velocity += acceleration * Mathf.Min(Time.deltaTime, 0.125f); // Sometimes, in an initial lag spike objects could have unrealistic amounts of acceleration applied

        // Calculate the new position based on velocity
        transform.position += velocity * Time.deltaTime;

        // Store the direction of motion and rotate the object to face it
        if (velocity.sqrMagnitude > 0.1f)
        {
            direction = velocity.normalized;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 1 - Mathf.Pow(1 - rotationLerp, Time.deltaTime));

        // Zero out acceleration for the next frame
        acceleration = Vector3.zero;
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
    /// When selected, draws a green wire sphere gizmo to represent this object's bounding circle
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
