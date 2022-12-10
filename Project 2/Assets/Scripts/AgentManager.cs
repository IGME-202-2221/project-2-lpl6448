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
    /// Agent prefab to spawn in on Awake
    /// </summary>
    public Agent elfPrefab;

    /// <summary>
    /// Number of agentPrefabs to spawn in on Awake
    /// </summary>
    public int numElves = 10;

    public Agent snowmanPrefab;

    public int numSnowmen = 5;

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

        for (int i = 0; i < numElves; i++)
        {
            agents.Add(SpawnElf(elfPrefab));
        }
        for (int i =0; i < numSnowmen; i++)
        {
            agents.Add(SpawnSnowman(snowmanPrefab));
        }
    }

    /// <summary>
    /// Instantiates the prefab at a random position within the elf boundaries
    /// </summary>
    /// <typeparam name="T">Type of Agent to spawn</typeparam>
    /// <param name="prefab">Prefab Agent that will be spawned in a random location in the world</param>
    /// <returns>The Agent (generic) that was spawned</returns>
    private T SpawnElf<T>(T prefab) where T : Agent
    {
        Vector3 pos = new Vector3(
            Random.Range(-WorldManager.Instance.elfWorldExtents.x, WorldManager.Instance.elfWorldExtents.x),
            0,
            Random.Range(-WorldManager.Instance.elfWorldExtents.z, WorldManager.Instance.elfWorldExtents.z));

        return Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    /// <summary>
    /// Instantiates the prefab at a random position within the snowman boundaries but outside the elf boundaries
    /// </summary>
    /// <typeparam name="T">Type of Agent to spawn</typeparam>
    /// <param name="prefab">Prefab Agent that will be spawned in a random location in the world</param>
    /// <returns>The Agent (generic) that was spawned</returns>
    private T SpawnSnowman<T>(T prefab) where T : Agent
    {
        Vector3 pos = Vector3.zero;
        while (pos.x > -WorldManager.Instance.elfWorldExtents.x
            && pos.x < WorldManager.Instance.elfWorldExtents.x
            && pos.z > -WorldManager.Instance.elfWorldExtents.z
            && pos.z < WorldManager.Instance.elfWorldExtents.z)
        {
            pos = new Vector3(
                Random.Range(-WorldManager.Instance.snowmanWorldExtents.x, WorldManager.Instance.snowmanWorldExtents.x),
                0,
                Random.Range(-WorldManager.Instance.snowmanWorldExtents.z, WorldManager.Instance.snowmanWorldExtents.z));
        }

        return Instantiate(prefab, pos, Quaternion.identity, transform);
    }
}