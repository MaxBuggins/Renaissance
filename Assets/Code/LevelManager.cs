using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    public float respawnDelay = 1;

    private List<Transform> spawnPoints = new List<Transform>();
    public List<Player> players = new List<Player>();

    private AudioSource audioSource;
    private AudioDistortionFilter audioDistortion;

    private UI_Main playerUI;
    private NetworkManagerHUD hud;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioDistortion = GetComponent<AudioDistortionFilter>();

        playerUI = FindObjectOfType<UI_Main>();
        hud = FindObjectOfType<NetworkManagerHUD>();

        if (hud != null) //TEMPARAY
            hud.showGUI = false;

        foreach(Transform child in transform)
        {
            if (child.tag == "SpawnPoint")
                spawnPoints.Add(child);
        }
    }

    private void Update()
    {
        if (audioDistortion != null)
        {
            if (playerUI.player.health <= 0)
            {
                audioDistortion.distortionLevel = 0.975f;
                audioSource.volume = 0.05f;
            }
            else
            {
                audioDistortion.distortionLevel = 0.1f;
                audioSource.volume = 0.5f;
            }
        }
    }

    [ClientRpc]
    public void sendKillMsg(string killer, string dier, HurtType hurtType)
    {
        playerUI.UIAddKillFeed(killer, dier, (int)hurtType);
    }

    public Transform GetSpawnPoint()
    {
        if (spawnPoints.Count <= 0)
            return (null);

        return (spawnPoints[Random.Range(0, spawnPoints.Count)]);
    }
}
