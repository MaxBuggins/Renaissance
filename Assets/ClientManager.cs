using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    public string playerName = "NoNameNed";
    public Color playerColour = Color.white;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

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
}
