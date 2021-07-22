using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    public float deActiveDelay = 0.8f;

    void Start()
    {
        Invoke(nameof(deActive), deActiveDelay);
    }

    
    void deActive()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
