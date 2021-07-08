using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoveWithPlatform : NetworkBehaviour
{
    private List<Player> inTrigger = new List<Player>();

    void Start()
    {
        if (isClient)
            enabled = false;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                inTrigger.Add(player);
            }
        }
    }
}
