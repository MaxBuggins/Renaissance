using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSquash : MonoBehaviour
{
    public float damagePerSpeed = 30;

    public Collider _collider;

    public Hurtful hurtful;
    private Player player;

    void Start()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (hurtful == null)
            hurtful = GetComponent<Hurtful>();

        player = GetComponentInParent<Player>();

        enabled = player.netIdentity.isServer;
    }

    void Update()
    {
        float fallSpeed = (player.transform.position.y - player.lastPos.y) * Time.deltaTime;
        fallSpeed *= 100;

        fallSpeed = Mathf.Abs(fallSpeed);

        hurtful.damage = (int)(fallSpeed * damagePerSpeed);

        if (hurtful.damage <= 10)
            hurtful.damage = 0;
    }
}
