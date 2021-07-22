using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering.Universal;

public class DestoryOnTrigger : MonoBehaviour
{
    public float destroyDelay = 0.5f;

    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb.gameObject);
            return;
        }
    }
}
