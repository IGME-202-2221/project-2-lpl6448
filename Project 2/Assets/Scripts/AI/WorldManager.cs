using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager class that contains information about the world, like the list of Stations.
/// 
/// Author: Luke Lepkowski (lpl6448@rit.edu)
/// </summary>
public class WorldManager : MonoBehaviour
{
    /// <summary>
    /// Singleton reference to the game's WorldManager instance.
    /// </summary>
    public static WorldManager Instance;

    /// <summary>
    /// List of Stations in the world, used to select viable destinations for items
    /// </summary>
    public List<Station> stations = new List<Station>();

    /// <summary>
    /// Initializes the Instance singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}