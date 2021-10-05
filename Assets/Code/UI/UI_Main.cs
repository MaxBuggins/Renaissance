using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Main : MonoBehaviour
{
    public Sprite[] deathSprites;
    public ObjectPlayerClass[] classes;

    public PlayerBase playerBase;
    [HideInInspector] public Player player;

    public UI_Base[] uiBases;

    public GameObject gameUI;
    public GameObject deathUI;
    public Image killerIcon;
    public Image killerLeter;
    public GameObject pauseUI;
    public GameObject classUI;

    public Image reloadRing;
    public Image classPreview;

    [HideInInspector] public LevelManager levelManager;
    [HideInInspector] public List<Player> players = new List<Player>();

    public Transform killFeed;
    public GameObject killLine;

    public UI_ClassDetails classDetails;

    public TMP_InputField msgBox;

    private Controls controls;
    private EventSystem eventSystem;


    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        eventSystem = FindObjectOfType<EventSystem>();

        controls = new Controls();

        controls.Game.Message.performed += HOWCANBEBEYONDFUNNY => MsgBox();

        controls.Enable();
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

        classPreview.sprite = classes[(int)player.playerClass.playerClass].classSprite;
    }

    public void UIAddKillFeed(string killer, string dier, int hurtType)
    {
        UI_KillLine kL = Instantiate(killLine, killFeed).GetComponent<UI_KillLine>();

        kL.killer = killer;
        kL.dier = dier;
        kL.killSprite = deathSprites[hurtType];
        kL.UpdateInfo();
    }

    public void OnDeathUI(bool active)
    {
        deathUI.SetActive(active);
        //killerIcon.sprite = classes[killerClassNum].classSprite;
        //killerLeter.sprite = classes[killerClassNum].classLeter;
    }

    public void Pause(bool pause)
    {
        pauseUI.SetActive(pause);
        classUI.SetActive(false);
        gameUI.SetActive(!pause);
    }

    public void MsgBox()
    {
        bool active = msgBox.IsActive();

        msgBox.gameObject.SetActive(!active);
        if (!active)
        {
            msgBox.Select();
            msgBox.text = "";
            player.controls.Disable();
        }
        else
        {
            msgBox.OnDeselect(null);
            player.controls.Enable();
        }
    }

    public void SendMsg(string msg)
    {
        print(msg);
    }

    public void DisplayClassDetails(int classNum)
    {
        classUI.SetActive(true);
        gameUI.SetActive(false);
        classDetails.DisplayDetails(classes[classNum]);
    }

    public void ChangeClass(int classNum)
    {
        playerBase.ChangeClass(classNum);
        classUI.SetActive(false);
        gameUI.SetActive(true);
        eventSystem.SetSelectedGameObject(null);
        if(player != null)
            player.Pause(false, 0);
    }
}
