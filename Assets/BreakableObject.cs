using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BreakableObject : NetworkBehaviour
{
    public float health = 30;
    public float damagePerSec = 1;

    public Transform pieces;
    public Sprite[] stateSprites;

    private Renderer render;

    private void Start()
    {
        if (!isServer)
            enabled = false;

        render = GetComponent<Renderer>();
    }

    [Server]
    void Update()
    {
        health -= damagePerSec * Time.deltaTime;


        //render.material.

        if (health <= 0)
        {
            DestoryObject();
        }

    }

    void Hurt(int damage)
    {
        health -= damage;
    }

    void DestoryObject()
    {
        RPCDestoryObject();
        pieces.parent = null;
        pieces.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    [ClientRpc]
    void RPCDestoryObject()
    {
        pieces.parent = null;
        pieces.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
