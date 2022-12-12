using UnityEngine;

/// <summary>
/// Represents a pile of snow that is created when a Snowball lands on the ground and acts
/// as an obstacle for Elves, until it slowly melts and disappears.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class SnowPile : MonoBehaviour
{
    /// <summary>
    /// Reference to the Obstacle that is automatically added to and removed from
    /// the list of Obstacles in ObstacleManager
    /// </summary>
    public Obstacle obstacle;

    /// <summary>
    /// Reference of the Transform for the SnowPile model, which slowly moves into
    /// the ground while melting
    /// </summary>
    public Transform modelTransform;

    /// <summary>
    /// Number of seconds that this SnowPile stays for before beginning to melt
    /// </summary>
    public float stayTime = 12;

    /// <summary>
    /// Number of seconds that this SnowPile takes to melt before disappearing
    /// </summary>
    public float meltTime = 2;

    /// <summary>
    /// Vertical height of this SnowPile, used to animate the modelTransform into
    /// the ground while melting
    /// </summary>
    public float height = 1;

    /// <summary>
    /// In-game time when this SnowPile is created, used to eventually melt and destroy this SnowPile
    /// </summary>
    private float startTime;

    /// <summary>
    /// Initializes the startTime and the modelTransform's random rotation
    /// </summary>
    private void Start()
    {
        startTime = Time.time;
        modelTransform.localEulerAngles = Vector3.up * Random.Range(0, 4) * 90;
    }

    /// <summary>
    /// Every frame, animates the modelTransform downward until destroying the SnowPile
    /// after stayTime + meltTime seconds
    /// </summary>
    private void Update()
    {
        float time = Time.time - startTime;
        if (time >= stayTime && time < stayTime + meltTime)
        {
            float t = (time - stayTime) / meltTime;
            Vector3 pos = modelTransform.localPosition;
            pos.y = -height * t;
            modelTransform.localPosition = pos;
        }
        else if (time >= stayTime + meltTime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Whenever the SnowPile is enabled, adds the obstacle to the list of Obstacles in ObstacleManager
    /// </summary>
    private void OnEnable()
    {
        ObstacleManager.Instance.obstacles.Add(obstacle);
    }

    /// <summary>
    /// Whenever the SnowPile is disabled, removes the obstacle from the list of Obstacles in ObstacleManager
    /// </summary>
    private void OnDisable()
    {
        ObstacleManager.Instance.obstacles.Remove(obstacle);
    }
}