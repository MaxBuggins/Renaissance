using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSquash : MonoBehaviour
{
    public float damagePerSpeed = 100;
    public float minFallTime = 0.3f;

    public Collider _collider;

    public Hurtful hurtful;
    public Player player;

    void Start()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (hurtful == null)
            hurtful = GetComponent<Hurtful>();

        if(player == null)
            player = GetComponentInParent<Player>();

        enabled = player.netIdentity.isServer;
    }

    [Server]
    void Update()
    {
        if (player.fallTime < minFallTime)
        {
            hurtful.damage = 0;
            return;
        }


        float fallDamage = Mathf.Pow(player.fallTime, 1.6f) * damagePerSpeed; //thanks desmos


        hurtful.damage = (int)(fallDamage);
    }
}
