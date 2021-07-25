using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlatform : MonoBehaviour
{

    private Vector3 lastPos;

    private Player playerInTrigger;

    void Start()
    {

    }

    void Update()
    {
        if (playerInTrigger != null)
        {
            playerInTrigger.velocity += (transform.position - lastPos) * 3; //times 3 to really feel the force
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
                }
            }
        }
    }
}
