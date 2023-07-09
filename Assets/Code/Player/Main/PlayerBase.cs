using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBase : Hurtable
{
    //public PlayerStats stats;

    [Header("Player Refrences")]
    private MyNetworkManager myNetworkManager;
    private ClientManager clientManager;
    private LevelManager levelManager;

    public UI_Main uIMain;

    public void Start()
    {
        myNetworkManager = FindObjectOfType<MyNetworkManager>();
        clientManager = FindObjectOfType<ClientManager>();
        levelManager = FindObjectOfType<LevelManager>();

        if (!netIdentity.isLocalPlayer)
            return;

        uIMain = FindObjectOfType<UI_Main>();
        uIMain.playerBase = this;
    }

    public void ChangeClass(int classObjIndex)
    {
        Player player = GetComponent<Player>();
        if (player != null)
        {
            if(player.playerWeapon != null)
                player.playerWeapon.controls.Dispose();
        }

        if(!netIdentity.isServer)
            CmdChangeClass(classObjIndex);
        else
            myNetworkManager.ChangePlayer(connectionToClient, myNetworkManager.spawnPrefabs[classObjIndex]);
    }

    [Client]
    public void Spectate()
    {
        //playerWeapon.controls.Dispose();

        CmdSpectate();
    }

    [Command]
    void CmdChangeClass(int classObjIndex)
    {
        myNetworkManager.ChangePlayer(connectionToClient, myNetworkManager.spawnPrefabs[classObjIndex]);
    }

    [Command]
    void CmdSpectate()
    {
        myNetworkManager.ChangePlayer(connectionToClient, myNetworkManager.playerPrefab);
    }

}