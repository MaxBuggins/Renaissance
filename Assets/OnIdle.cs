using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OnIdle : NetworkBehaviour
{
    private Player player;

    private float idleVelocity = 0.25f;

    public float initalDelay = 2;
    public float idleDelay = 0.1f;
    private float timeSinceMovement = 0;

    void Start()
    {
        enabled = false;
        if (isServer)
            Invoke(nameof(Enable), initalDelay);

        player = GetComponent<Player>();
    }

    void Update()
    {
        if (player.velocity.magnitude < 0.5f)
            timeSinceMovement += Time.deltaTime;

        if (timeSinceMovement > idleDelay)
        {
            player.Hurt(1);
            timeSinceMovement = 0;
        }
    }

    private void Enable()
    {
        enabled = true;
    }
}
