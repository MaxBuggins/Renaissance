using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    [Range(0,2)] public float damageMultiplyer = 1;

    public Player player;

    public Collider theCollider;

    private void Start()
    {
        if (player.isLocalPlayer)
            theCollider.enabled = false;
    }
}
