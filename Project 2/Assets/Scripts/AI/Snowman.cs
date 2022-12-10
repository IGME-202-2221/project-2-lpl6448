using UnityEngine;

public class Snowman : Agent
{
    public enum SnowmanState
    {
        Wandering,
        Throwing
    }

    public Animator animator;

    public MaterialColor modelColor;

    public Snowball snowballPrefab;

    public Color throwColor = Color.red;

    public float throwTime = 1;

    public float throwStateDuration = 2;

    public float throwAngle = 60;

    public Transform throwFrom;

    public float throwChancePerSecond = 0.2f;

    public float throwCooldown = 3;

    public SnowmanState state = SnowmanState.Wandering;

    private float stateStartTime;

    private Vector3 throwGoal;

    private bool hasThrown = false;

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