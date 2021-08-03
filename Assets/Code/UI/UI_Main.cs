using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    public Sprite[] deathSprites;
    public Sprite[] classSprites;


    [HideInInspector] public Player player;

    public UI_Base[] uiBases;
    public GameObject deathUI;
    public GameObject pauseUI;
    public Image reloadRing;
    public Image classPreview;

    [HideInInspector] public LevelManager levelManager;
    [HideInInspector] public List<Player> players = new List<Player>();

    public Transform killFeed;
    public GameObject killLine;


    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Update()
    {
        if(player != null)
            reloadRing.fillAmount = player.playerWeapon.reloadPerstenage;
    }

    public void UIUpdate()
    {
        players.Clear();
        foreach (Player player in FindObjectsOfType<Player>()) //sort players by score
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

        classPreview.sprite = classSprites[(int)player.playerClass.playerClass];
        deathUI.SetActive(player.health <= 0); //bit of a coder
    }

    public void UIAddKillFeed(string killer, string dier, int hurtType)
    {
        UI_KillLine kL = Instantiate(killLine, killFeed).GetComponent<UI_KillLine>();

        kL.killer = killer;
        kL.dier = dier;
        kL.killSprite = deathSprites[hurtType];
        kL.UpdateInfo();
    }

    public void Pause(bool pause)
    {
        pauseUI.SetActive(pause);
    }
}
