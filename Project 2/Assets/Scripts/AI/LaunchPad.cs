using UnityEngine;
using System.Collections;

public class LaunchPad : MonoBehaviour
{
    public float prepareOffset = -0.2f;

    public float prepareSpring = 10;

    public float prepareDamper = 2;

    public float prepareDuration = 1.5f;

    public float launchAcceleration = 16;

    public float launchSpeed = 8;

    public float timeToDespawn = 5;

    public Transform attachedItemContainer;

    private float position = 0;

    private float velocity = 0;

    public void AttachItem(ItemType item)
    {
        ItemObject itemObj = Instantiate(item.objectPrefab, attachedItemContainer, false);
        itemObj.StackObject(0);
    }

    public void Launch()
    {
        StartCoroutine(LaunchCrt());
    }

    private void LateUpdate()
    {
        position += velocity * Time.deltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x, position, transform.localPosition.z);
    }

    private IEnumerator LaunchCrt()
    {
        // Prepare to launch
        float startTime = Time.time;
        while (Time.time - startTime < prepareDuration)
        {
            velocity += (prepareOffset - position) * prepareSpring * Time.deltaTime;
            velocity *= 1 - Mathf.Clamp01(prepareDamper * Time.deltaTime);

            yield return null;
        }
        velocity = 0;

        // Launch
        while (velocity < launchSpeed)
        {
            velocity += launchAcceleration * Time.deltaTime;

            yield return null;
        }
        velocity = launchSpeed;

        // Despawn
        yield return new WaitForSeconds(timeToDespawn);
        Destroy(gameObject);
    }
}