using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    private AudioSource audioSource;
    private Player player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GetComponentInParent<Player>();
    }

    void Update()
    {
        if (player.fallTime < player.coyotTime && player.move.magnitude != 0)
        {
            audioSource.volume = 1;
        }
        else
        {
            audioSource.volume = 0;
        }

    }
}
