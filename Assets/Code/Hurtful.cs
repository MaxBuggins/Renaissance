using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hurtful : NetworkBehaviour
{
    public int damage = 1;

    public bool destoryRigidbodys = false;
    public float destroyDelay;


    void Awake()
    {
        if (isClient) //only for the server to run
            enabled = false;
    }
    
    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
                player.health -= damage;
            return;
        }

        if (!destoryRigidbodys)
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
            StartCoroutine(WaitThenDestory(rb.gameObject));
    }

    [Server]

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.health -= damage;
            return;
        }

        if (!destoryRigidbodys)
            return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
            StartCoroutine(WaitThenDestory(rb.gameObject));
    }

    IEnumerator WaitThenDestory(GameObject obj)
    {
        yield return new WaitForSeconds(destroyDelay);
        NetworkServer.Destroy(obj);
    }
}
