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
    /// Extents of a bounding box describing the region where Elves can navigate
    /// but that Snowmen cannot enter
    /// </summary>
    public Vector3 elfWorldExtents;

    /// <summary>
    /// Extents of a bounding box describing the region where Snowmen can navigate
    /// </summary>
    public Vector3 snowmanWorldExtents;

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

    /// <summary>
    /// When selected, draws two yellow wire cubes to represent the world bounds
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, elfWorldExtents * 2);
        Gizmos.DrawWireCube(Vector3.zero, snowmanWorldExtents * 2);
    }
}