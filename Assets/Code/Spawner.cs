using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour
{
    public Transform[] spawnPoints;
    public Vector2 spawnDelay;

    public GameObject gObject;

    void Start()
    {
        if (spawnPoints.Length < 1)
        {
            spawnPoints = new Transform[1];
            spawnPoints[0] = transform;
        }

            Invoke(nameof(SpawnObject), Random.Range(spawnDelay.x, spawnDelay.y));
    }

    void SpawnObject()
    {
        Transform trans = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var spawn = Instantiate(gObject, trans.position, trans.rotation);
        
     
        NetworkServer.Spawn(spawn);

        //repeat
        Invoke(nameof(SpawnObject), Random.Range(spawnDelay.x, spawnDelay.y));
    }

}
