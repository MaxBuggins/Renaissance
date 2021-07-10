using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    public Player player;

    public UI_Base[] uiBases;
    public GameObject deathUI;

    public LevelManager levelManager;
    public List<Player> players = new List<Player>();

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void UIUpdate()
    {
        players.Clear();
        foreach (Player player in FindObjectsOfType<Player>())
        {
            for(int i = 0; i < players.Count; i ++)
            {
                if (player.score > players[i].score)
                {
                    players.Insert(i, player);
                    break;
                }
            }
            players.Add(player);

        }

        foreach (UI_Base ui in uiBases)
            ui.UpdateInfo();


        deathUI.SetActive(player.health <= 0); //bit of a coder
    }
}
