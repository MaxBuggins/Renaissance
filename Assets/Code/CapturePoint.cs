using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CapturePoint : MonoBehaviour
{
    public float pointDelay = 0.5f;
    private float pointTime;

    public List<Player> playersIn = new List<Player>();

    [ClientCallback]
    void Start()
    {
        enabled = false;
    }

    [Server]
    void Update()
    {
        pointTime += Time.deltaTime;

        if(pointTime > pointDelay)
        {
            foreach (Player player in playersIn)
            {
                if (player.health > 0)
                    player.addScore(1);
                else
                    playersIn.Remove(player);
            }

            pointTime = 0;
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            playersIn.Add(player);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            playersIn.Remove(player);
        }
    }
}
