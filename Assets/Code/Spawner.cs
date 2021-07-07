using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Spawner : NetworkBehaviour
{
    public float spawnDelay;

    public GameObject gObject;

    void Start()
    {
        Invoke(nameof(SpawnObject), spawnDelay);
    }

    void SpawnObject()
    {
        var spawn = Instantiate(gObject, transform.position, transform.rotation);
        NetworkServer.Spawn(spawn);

        //repeat
        Invoke(nameof(SpawnObject), spawnDelay);
    }

}
