using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager class that contains references to all Agents in the scene and some
/// shared fields that each Agent needs.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class AgentManager : MonoBehaviour
{
    /// <summary>
    /// Singleton AgentManager in the scene
    /// </summary>
    public static AgentManager Instance;

    /// <summary>
    /// Units of padding of the world boundaries from the camera area
    /// </summary>
    public float edgePadding = 1;

    /// <summary>
    /// Agent prefab to spawn in on Awake
    /// </summary>
    public Agent agentPrefab;

    /// <summary>
    /// Number of agentPrefabs to spawn in on Awake
    /// </summary>
    public int numAgents = 10;

    /// <summary>
    /// Minimum (lower-left) corner of the world boundaries
    /// </summary>
    [HideInInspector]
    public Vector3 minPosition = -Vector3.one;

    /// <summary>
    /// Maximum (upper-right) corner of the world boundaries
    /// </summary>
    [HideInInspector]
    public Vector3 maxPosition = Vector3.one;

    /// <summary>
    /// List of Agents in the scene, used for separate forces
    /// </summary>
    public List<Agent> agents = new List<Agent>();

    /// <summary>
    /// To initialize, update the singleton, set the world boundaries, and spawn Agents
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector2 camPosition = cam.transform.position;
            Vector2 camExtents = new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize) - Vector2.one * edgePadding;
            minPosition = camPosition - camExtents;
            maxPosition = camPosition + camExtents;
        }

        for (int i = 0; i < numAgents; i++)
        {
            agents.Add(Spawn(agentPrefab));
        }
    }

    /// <summary>
    /// Instantiates the prefab at a random position within the world boundaries
    /// </summary>
    /// <typeparam name="T">Type of Agent to spawn</typeparam>
    /// <param name="prefab">Prefab Agent that will be spawned in a random location in the world</param>
    /// <returns>The Agent (generic) that was spawned</returns>
    private T Spawn<T>(T prefab) where T : Agent
    {
        Vector2 pos = new Vector2(
            Random.Range(minPosition.x, maxPosition.x),
            Random.Range(minPosition.y, maxPosition.y));

        return Instantiate(prefab, pos, Quaternion.identity);
    }
}