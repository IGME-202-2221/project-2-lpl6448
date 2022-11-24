using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    public List<Station> stations = new List<Station>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}