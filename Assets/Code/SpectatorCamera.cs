using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpectatorCamera : NetworkBehaviour
{
    private Controls controls;

    private NetworkIdentity identity;
    private MyNetworkManager networkManager;

    void Start()
    {
        identity = GetComponent<NetworkIdentity>();
        networkManager = FindObjectOfType<MyNetworkManager>();

        controls = new Controls();

        controls.Game.ChangeClass.performed += funnyiest => ChangeClass(Random.Range(0, 3));

        controls.Enable();
    }


    void Update()
    {
    }


    public void ChangeClass(int prefabIndex)
    {
        networkManager.ChangePlayer(identity.connectionToClient, networkManager.spawnPrefabs[prefabIndex]);
    }
}
