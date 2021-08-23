using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering.Universal;

public class DestoryOnTrigger : MonoBehaviour
{
    public float destroyDelay = 0.5f;

    public GameObject triggerPrefab;

    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(triggerPrefab != null)
        {
            Instantiate(triggerPrefab, other.transform.position + Vector3.up * 0.75f, triggerPrefab.transform.rotation, null);
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb.gameObject);
            return;
        }
    }
}
