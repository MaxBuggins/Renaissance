using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientManager : MonoBehaviour
{
    public string playerName = "NoNameNed";
    public Color playerColour = Color.white;

    public PlayerClass playerClass = PlayerClass.Convict;

    public string selectedMap = "MainMenu";

    private MyNetworkManager netManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        netManager = FindObjectOfType<MyNetworkManager>();

        playerName = "NoNameNed" + Random.Range(0, 99);
        playerColour = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    void Update()
    {
    }

    public void UpdateInfo(string newName = default)
    {
        if(playerName != default)
            playerName = newName;

        //if (playerColour != default)
            //playerColour = newColour;
    }

    public void UpdateMap(string mapName)
    {
        selectedMap = mapName;
        netManager.onlineScene = selectedMap; 
    }

}
