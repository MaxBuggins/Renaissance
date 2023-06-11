using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{
    public GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        base.Awake();
    }

    public override void OnClientConnect()
    {

        //if (hud != null) //TEMPARAY
        //hud.showGUI = false;

        base.OnClientConnect();
    }

    public override void OnClientDisconnect()
    {
        //if (hud != null) //TEMPARAY
        //hud.showGUI = true;

        Cursor.lockState = CursorLockMode.None;

        base.OnClientDisconnect();
    }

    //public override void OnServerConnect(NetworkConnection conn)
    //{
        //conn.identity.gameObject.AddComponent<PlayerStats>();
    //}

        //public override void OnServerDisconnect(NetworkConnection conn)
        //{

            //gameManager.players.Remove(conn.connectionId);

        //}

    public void ChangePlayer(NetworkConnection conn, GameObject newPrefab)
    {
        if (conn == null)
        {
            print("No Connection for Change Player");
            return;
        }

        if (conn.owned.Count <= 0)
        {
            // Instantiate the new player object and broadcast to clients
            NetworkServer.ReplacePlayerForConnection(conn.identity.connectionToClient, Instantiate(newPrefab));
            return;
        }

        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;
        PlayerStats oldPlayerStats = conn.identity.GetComponent<PlayerStats>();

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(conn.identity.connectionToClient, Instantiate(newPrefab));
        PlayerStats newPlayerStats = conn.identity.GetComponent<PlayerStats>();

        if (newPlayerStats != null && oldPlayerStats != null)
        {
            newPlayerStats.userName = oldPlayerStats.userName;
            newPlayerStats.colour = oldPlayerStats.colour;
            newPlayerStats.kills = oldPlayerStats.kills;
            newPlayerStats.killStreak = oldPlayerStats.killStreak;
            newPlayerStats.assists = oldPlayerStats.assists;
            newPlayerStats.deaths = oldPlayerStats.deaths;
            newPlayerStats.bonusScore = oldPlayerStats.bonusScore;
        }

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);
    }
}
