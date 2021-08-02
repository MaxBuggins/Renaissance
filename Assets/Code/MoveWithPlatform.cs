using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlatform : MonoBehaviour
{

    private Vector3 velocity;
    private Vector3 lastPos;

    private Player playerInTrigger;

    int i = 0;

    void Start()
    {

    }

    void FixedUpdate()
    {
        velocity = transform.position - lastPos;

        if (playerInTrigger != null)
        {
            if (playerInTrigger.health <= 0)
            {
                playerInTrigger = null;
                return;
            }

            Vector3 relativeVelocity = (velocity / Time.fixedDeltaTime) * 0.09f; //not sure why 0.09

            playerInTrigger.velocity += relativeVelocity;

        }

        lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                if (player.isLocalPlayer)
                {
                    playerInTrigger = player;
                    //player.transform.parent = transform;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                if (player.isLocalPlayer)
                {
                    playerInTrigger = null;
                    //player.transform.parent = null;
                    //manually add velocity after leveing trigger
                    //player.velocity += velocity / Time.fixedDeltaTime;
                }
            }
        }
    }
}
