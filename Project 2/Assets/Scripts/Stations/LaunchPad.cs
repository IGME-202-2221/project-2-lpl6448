using UnityEngine;
using System.Collections;

/// <summary>
/// Represents an anchor for finished ItemObjects and launches them to Santa.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class LaunchPad : MonoBehaviour
{
    /// <summary>
    /// Vertical positional offset that the LaunchPad approaches before launching
    /// </summary>
    public float prepareOffset = -0.2f;

    /// <summary>
    /// Spring used to move the LaunchPad toward the prepareOffset before launching
    /// </summary>
    public float prepareSpring = 10;

    /// <summary>
    /// Damper used on the velocity when moving the LaunchPad before launching
    /// </summary>
    public float prepareDamper = 2;

    /// <summary>
    /// Number of seconds to move the LaunchPad toward the prepareOffset before launching
    /// </summary>
    public float prepareDuration = 1.5f;

    /// <summary>
    /// Upward acceleration of the LaunchPad when beginning to launch
    /// </summary>
    public float launchAcceleration = 16;

    /// <summary>
    /// Maximum (terminal) vertical speed that the LaunchPad can reach while accelerating upward
    /// </summary>
    public float launchSpeed = 8;

    /// <summary>
    /// Number of seconds after launching before the LaunchPad is destroyed
    /// </summary>
    public float timeToDespawn = 5;

    /// <summary>
    /// Container/parent Transform for the attached ItemObject that is being launched
    /// </summary>
    public Transform attachedItemContainer;

    /// <summary>
    /// Current vertical local position of this LaunchPad
    /// </summary>
    private float position = 0;

    /// <summary>
    /// Current vertical velocity of this LaunchPad
    /// </summary>
    private float velocity = 0;

    /// <summary>
    /// Instantiates the item's ItemObject in the attachedItemContainer to launch it
    /// </summary>
    /// <param name="item">ItemType to place on the LaunchPad</param>
    public void AttachItem(ItemType item)
    {
        ItemObject itemObj = Instantiate(item.objectPrefab, attachedItemContainer, false);
        itemObj.StackObject(0);
    }

    /// <summary>
    /// Begins the launch process for this LaunchPad
    /// </summary>
    public void Launch()
    {
        StartCoroutine(LaunchCrt());
    }

    /// <summary>
    /// Briefly moves the LaunchPad down to simulate a spring, launches the LaunchPad
    /// (and any attached ItemObject) into the sky, and eventually destroys this LaunchPad
    /// </summary>
    /// <returns>IEnumerator for this coroutine</returns>
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

    /// <summary>
    /// Every frame, updates the position based on the velocity
    /// </summary>
    private void LateUpdate()
    {
        position += velocity * Time.deltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x, position, transform.localPosition.z);
    }
}