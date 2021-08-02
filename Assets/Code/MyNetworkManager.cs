using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetworkManager : NetworkManager
{

    public void ChangePlayer(NetworkConnection conn, GameObject newPrefab)
    {
        if (conn == null)
        {
            print("No Connection for Change Player");
            return;
        }

        if(conn.clientOwnedObjects.Count <= 0)
        {   
            // Instantiate the new player object and broadcast to clients
            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab));
            return;
        }

        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);
    }
}
