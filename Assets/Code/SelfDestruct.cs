using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SelfDestruct : MonoBehaviour
{
    public bool unParentOnStart = false;

    public float destroyDelay = 1f;
    public bool destoryOnServer = false;

    void Start()
    {
        if (unParentOnStart == true)
            transform.parent = null;

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
