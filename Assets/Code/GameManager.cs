using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    //public readonly SyncDictionary<string, PlayerStats> players = new SyncDictionary<string, PlayerStats>(); //Server Only

    public MyNetworkManager netManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //netManager.gameManager = this;
    }

    void Update()
    {

    }
}
