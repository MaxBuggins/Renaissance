using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpectatorCamera : NetworkBehaviour
{
    private NetworkIdentity identity;
    private MyNetworkManager networkManager;

    void Start()
    {
        identity = GetComponent<NetworkIdentity>();
        networkManager = FindObjectOfType<MyNetworkManager>();
    }


    void Update()
    {
        ChangeClass(1);
    }


    public void ChangeClass(int prefabIndex)
    {
        networkManager.ChangePlayer(identity.connectionToClient, networkManager.spawnPrefabs[prefabIndex]);
    }
}
