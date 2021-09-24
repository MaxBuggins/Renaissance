using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class BreakableObject : NetworkBehaviour
{
    public bool scaleOverTime = false;
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;
    public float scaleingDuration = 1.25f;
    public AnimationCurve scaleCurve;

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

        if (scaleOverTime)
        {
            transform.localScale = startScale;
            Tween.LocalScale(transform, endScale, scaleingDuration, 0, scaleCurve);
        }
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
