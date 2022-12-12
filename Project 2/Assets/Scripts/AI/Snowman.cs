using UnityEngine;

/// <summary>
/// Represents a Snowman that wanders around the map and occasionally throws Snowballs
/// into the workshop. Also contains visible state, such as the Animator.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class Snowman : Agent
{
    /// <summary>
    /// Contains the two states that a Snowman can be in.
    /// Wandering: The Snowman is wandering around the bounds of the map.
    /// Throwing: The Snowman is preparing or has just thrown a Snowball.
    /// </summary>
    public enum SnowmanState
    {
        Wandering,
        Throwing
    }

    /// <summary>
    /// Reference to the Animator, used for a walking animation
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Reference to the model's MaterialColor component, used to alter the color of this individual Snowman
    /// </summary>
    public MaterialColor modelColor;

    /// <summary>
    /// Snowball prefab to instantiate when throwing
    /// </summary>
    public Snowball snowballPrefab;

    /// <summary>
    /// Color to change the model when in the Throwing state
    /// </summary>
    public Color throwColor = Color.red;

    /// <summary>
    /// Amount of time after entering the throwing animation before the Snowball is instantiated and visibly thrown
    /// </summary>
    public float throwTime = 1;

    /// <summary>
    /// Total amount of time spent in the Throwing state before reverting to Wandering
    /// </summary>
    public float throwStateDuration = 2;

    /// <summary>
    /// Angle (degrees) from the xy-plane to throw the Snowball at
    /// </summary>
    public float throwAngle = 60;

    /// <summary>
    /// Positional reference to initially instantiate the Snowball at
    /// </summary>
    public Transform throwFrom;

    /// <summary>
    /// Probability (0-1) per second that this Snowman will enter the Throwing state
    /// </summary>
    public float throwChancePerSecond = 0.2f;

    /// <summary>
    /// Minimum number of seconds this Snowman must wander before throwing another Snowball
    /// </summary>
    public float throwCooldown = 3;

    /// <summary>
    /// Current SnowmanState of this Snowman
    /// </summary>
    public SnowmanState state = SnowmanState.Wandering;

    /// <summary>
    /// In-game time when the state was most recently changed
    /// </summary>
    private float stateStartTime;

    /// <summary>
    /// World-space position where this Snowman will throw a Snowball
    /// </summary>
    private Vector3 throwGoal;

    /// <summary>
    /// If this Snowman is in the Throwing state, whether it has thrown a Snowball yet or not
    /// </summary>
    private bool hasThrown = false;

    /// <summary>
    /// Called every frame to calculate steering forces for this Snowman and for additional logic,
    /// like updating the animator and doing logic depending on the SnowmanState
    /// </summary>
    protected override void CalculateSteeringForces()
    {
        switch (state)
        {
            case SnowmanState.Wandering:
                Wander(1);
                AvoidAllObstacles(2);
                Separate(AgentManager.Instance.agents, 2);
                StayInBounds(-WorldManager.Instance.snowmanWorldExtents, WorldManager.Instance.snowmanWorldExtents);
                StayOutOfBounds(-WorldManager.Instance.elfWorldExtents, WorldManager.Instance.elfWorldExtents, 2);

                if (Time.time - stateStartTime > throwCooldown)
                {
                    float throwChancePerFrame = 1 - Mathf.Pow(1 - throwChancePerSecond, Time.deltaTime);
                    if (Random.value < throwChancePerFrame)
                    {
                        SetState(SnowmanState.Throwing);
                    }
                }
                break;
            case SnowmanState.Throwing:
                Stop(1);
                physicsObject.SetDirection((throwGoal - physicsObject.Position).normalized);

                if (Time.time - stateStartTime >= throwTime && !hasThrown)
                {
                    Throw();
                    hasThrown = true;
                }
                else if (Time.time - stateStartTime >= throwStateDuration)
                {
                    SetState(SnowmanState.Wandering);
                }
                break;
        }

        animator.SetFloat("WalkSpeed", physicsObject.Velocity.magnitude);
    }

    /// <summary>
    /// Changes the state to newState and performs any specific logic
    /// depending on the newState
    /// </summary>
    /// <param name="newState">SnowmanState to switch to</param>
    private void SetState(SnowmanState newState)
    {
        if (state != newState)
        {
            state = newState;
            stateStartTime = Time.time;

            switch (state)
            {
                case SnowmanState.Wandering:
                    modelColor.SetColor(Color.white);
                    break;
                case SnowmanState.Throwing:
                    modelColor.SetColor(throwColor);
                    animator.SetTrigger("Throw");

                    // Choose random throw goal
                    throwGoal = new Vector3(
                        Random.Range(-WorldManager.Instance.elfWorldExtents.x, WorldManager.Instance.elfWorldExtents.x),
                        0,
                        Random.Range(-WorldManager.Instance.elfWorldExtents.z, WorldManager.Instance.elfWorldExtents.z));
                    hasThrown = false;
                    break;
            }
        }
    }

    /// <summary>
    /// Instantiates a Snowball at the throwFrom position with the velocity required to land on the throwGoal
    /// </summary>
    private void Throw()
    {
        Vector3 throwOffset = throwGoal - throwFrom.position;

        float horizontalDis = new Vector3(throwOffset.x, 0, throwOffset.z).magnitude;
        Vector3 horizontalDir = new Vector3(throwOffset.x, 0, throwOffset.z) / horizontalDis;
        float verticalDis = throwOffset.y;

        float throwAngleTan = Mathf.Tan(throwAngle * Mathf.Deg2Rad);
        float horizontalSpeed = horizontalDis * Mathf.Sqrt(-snowballPrefab.gravity.y / 2 / (horizontalDis * throwAngleTan - verticalDis));
        float verticalSpeed = horizontalSpeed * throwAngleTan;
        Vector3 velocity = horizontalDir * horizontalSpeed + Vector3.up * verticalSpeed;

        Snowball snowball = Instantiate(snowballPrefab, throwFrom.position, Quaternion.identity);
        snowball.velocity = velocity;
    }
}