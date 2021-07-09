using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelfDestruct : MonoBehaviour
{
    public float destroyDelay = 1f;
    public bool destoryOnServer = false;

    void Start()
    {
        Invoke(nameof(DestroySelf), destroyDelay);
    }

    void DestroySelf()
    {
        if(destoryOnServer == false)
            Destroy(gameObject);
        else
            NetworkServer.Destroy(gameObject);
    }
}
