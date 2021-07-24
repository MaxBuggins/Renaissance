using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    public float deActiveDelay = 0.8f;
    public float minVelocity = 0.1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Invoke(nameof(deActive), deActiveDelay);
    }

    
    void deActive()
    {
        if (Mathf.Abs(rb.velocity.magnitude) > minVelocity)
        {
            Invoke(nameof(deActive), deActiveDelay / 2);
            return;
        }

        transform.DetachChildren();
        Destroy(gameObject);
    }
}
