using UnityEngine;

public class Snowball : MonoBehaviour
{
    public Obstacle landObstacle;

    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    
    public Vector3 velocity = Vector3.zero;

    private void Update()
    {
        velocity += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        if (transform.position.y <= 0)
        {
            Land();
        }
    }

    private void Land()
    {
        Instantiate(landObstacle, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        Destroy(gameObject);
    }
}