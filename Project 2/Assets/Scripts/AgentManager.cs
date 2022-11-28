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
    public Vector3 minPosition = -Vector3.one;

    /// <summary>
    /// Maximum (upper-right) corner of the world boundaries
    /// </summary>
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
        Vector3 pos = new Vector3(
            Random.Range(minPosition.x, maxPosition.x),
            0,
            Random.Range(minPosition.z, maxPosition.z));

        return Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    /// <summary>
    /// When selected, draws a yellow wire cube to represent the world bounds
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((minPosition + maxPosition) / 2, maxPosition - minPosition);
    }
}