using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBase : NetworkBehaviour
{
    [Header("Player Refrences")]
    private MyNetworkManager myNetworkManager;
    private ClientManager clientManager;
    private LevelManager levelManager;

    public UI_Main uIMain;

    void Awake()
    {
        myNetworkManager = FindObjectOfType<MyNetworkManager>();
        clientManager = FindObjectOfType<ClientManager>();
        levelManager = FindObjectOfType<LevelManager>();

        //if (!netIdentity.isLocalPlayer)
            //return;

        uIMain = FindObjectOfType<UI_Main>();
        uIMain.playerBase = this;
    }

    [Client]
    public void ChangeClass(int classObjIndex)
    {
        Player player = GetComponent<Player>();
        if (player != null)
            player.playerWeapon.controls.Dispose();

        CmdChangeClass(classObjIndex);
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
