using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Destroy(gameObject);
    }
}
