using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    public float respawnDelay = 1;

    private List<Transform> spawnPoints = new List<Transform>();
    public List<Player> players = new List<Player>();

    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.tag == "SpawnPoint")
                spawnPoints.Add(child);
        }
    }

    private void Update()
    {

    }

    public Vector3 GetSpawnPoint()
    {
        if (spawnPoints.Count <= 0)
            return (Vector3.zero);

        return (spawnPoints[Random.Range(0, spawnPoints.Count)].position);
    }
}
