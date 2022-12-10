using UnityEngine;

public class SnowPile : MonoBehaviour
{
    public Obstacle obstacle;

    public Transform modelTransform;

    public float stayTime = 12;

    public float meltTime = 2;

    public float height = 1;

    private float startTime;

    private void Start()
    {
        startTime = Time.time;
        modelTransform.localEulerAngles = Vector3.up * Random.Range(0, 4) * 90;
    }

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

    private void OnEnable()
    {
        ObstacleManager.Instance.obstacles.Add(obstacle);
    }
    private void OnDisable()
    {
        ObstacleManager.Instance.obstacles.Remove(obstacle);
    }
}