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
    
        Vector3 newVelocity = transform.position - lastPos;
/*
        Vector3 velDiffrence = (velocity - newVelocity);

        print(velDiffrence);

        if (playerInTrigger != null)
        {
            if (Mathf.Abs(velocity.x - newVelocity.x) > 0.1f ||
                Mathf.Abs(velocity.y - newVelocity.y) > 0.1f ||
                Mathf.Abs(velocity.z - newVelocity.z) > 0.1f)
            {
                playerInTrigger.velocity += (velDiffrence) / Time.fixedDeltaTime;
            }
        }
*/
        velocity = newVelocity;

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
                    player.transform.parent = transform;
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
                    player.transform.parent = null;
                    //manually add velocity after leveing trigger
                    player.velocity += velocity / Time.fixedDeltaTime;
                }
            }
        }
    }
}
