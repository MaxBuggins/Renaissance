using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : NetworkBehaviour //player details to keep track off even after the game
{
    [SyncVar]
    public string userName = "ERROR";
    [SyncVar]
    public Color32 colour = Color.black;

    [SyncVar]
    public int kills = 0; //umm no idear what this could mean
    [SyncVar]
    public int killStreak = 0; //how many kills before you respawn

    [SyncVar]
    public int assists = 0; //if you were helpful in someones death

    [SyncVar]
    public int deaths = 0; //you die you death

    [SyncVar(hook = nameof(OnScoreChange))]
    public int bonusScore = 0; //For gameMode unique scores like capturing a flag

    [Header("UnityStuff")]
    private UI_Main uI_Main;
    [HideInInspector] public Player player;

    private void Awake()
    {
        uI_Main = FindObjectOfType<UI_Main>();
        player = GetComponent<Player>();
    }

    [Command]
    public void CmdSetUpPlayer(string name, Color32 _colour)
    {
        userName = name;
        colour = _colour;
    }

    void OnScoreChange(int _Old, int _New)
    {
        uI_Main.UIUpdate();
    }

    public int GetScore() //common conversion for main score
    {
        return (kills * 5 + assists * 2 + deaths * -1 + bonusScore);
    }
}
