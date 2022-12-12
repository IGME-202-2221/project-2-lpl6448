using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager class that stores references to all Obstacles in the scene
/// and contains a singleton that can be used by other scripts to access the
/// list of obstacles.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class ObstacleManager : MonoBehaviour
{
    /// <summary>
    /// Singleton ObstacleManager in the scene
    /// </summary>
    public static ObstacleManager Instance;

    /// <summary>
    /// List of all Obstacles currently in the world
    /// </summary>
    public List<Obstacle> obstacles = new List<Obstacle>();

    /// <summary>
    /// To initialize, update the singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}