using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlatform : MonoBehaviour
{

    private float velocityMultiplyer = 0.078f;
    private Vector3 lastPos;

    private Player playerInTrigger;

    int i = 0;

    void Start()
    {

    }

    void FixedUpdate()
    {
        //i++;
        //if (playerInTrigger != null)
        //{
          //  print("triggerd " + i);

            //Vector3 magatitude = (transform.position - lastPos);

            //playerInTrigger.exsternalForce += magatitude * 1f;
        //}

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
                    player.velocity += (transform.position - lastPos) / Time.fixedDeltaTime;
                }
            }
        }
    }
}
