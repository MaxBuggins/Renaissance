using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float respawnDelay = 1;

    private List<Transform> spawnPoints = new List<Transform>();

    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.tag == "SpawnPoint")
                spawnPoints.Add(child);
        }
    }


    public Vector3 GetSpawnPoint()
    {
        if (spawnPoints.Count <= 0)
            return (Vector3.zero);

        return (spawnPoints[Random.Range(0, spawnPoints.Count)].position);
    }
}
