using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPlayer : OnCall
{
    public float pullForce;


    public override void Call(Player player)
    {
        player.TargetAddVelocity(player.connectionToClient, (transform.position - player.transform.position) * pullForce);

        base.Call(player);
    }
}
