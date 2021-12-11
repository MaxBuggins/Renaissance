using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : NetworkBehaviour //player details to keep track off even after the game
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string userName = "ERROR";
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color32 colour = Color.black;

    [SyncVar]
    public int kills = 0; //umm no idear what this could mean
    [SyncVar]
    public int killStreak = 0; //how many kills before you respawn

    [SyncVar]
    public int assists = 0; //if you were helpful in someones death

    [SyncVar]
    public int deaths = 0; //you die you death

    [SyncVar]
    public int bonusScore = 0; //For gameMode unique scores like capturing a flag

    [Header("UnityStuff")]
    private UI_Main uI_Main;

    private void Awake()
    {
        uI_Main = FindObjectOfType<UI_Main>();
    }

    void OnNameChanged(string _Old, string _New)
    {
        //playerAbove.playerNameText.text = playerName;
        if (isLocalPlayer)
            uI_Main.UIUpdate();
    }

    void OnColorChanged(Color32 _Old, Color32 _New) //fixed colours to 32 bits 0-255 int, while listening to Miitopia soundtrack 
    {
        //playerAbove.playerNameText.color = _New;
        if (isLocalPlayer)
            uI_Main.UIUpdate();
        //playerMaterialClone = new Material(GetComponent<Renderer>().material);
        //playerMaterialClone.color = _New;
        //GetComponent<Renderer>().material = playerMaterialClone;
    }


    public int GetScore() //common conversion for main score
    {
        return (kills * 3 + assists + deaths * -1 + bonusScore);
    }

}
