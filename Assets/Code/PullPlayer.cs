using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPlayer : OnCall
{
    public float pullForce;
    public float addUpForce = 1;


    public override void Call(Player player)
    {

        Vector3 force = (transform.position - player.transform.position) * pullForce;

        float yDist = (transform.position - player.transform.position).y;

        yDist += addUpForce;

        float upForce = Mathf.Sqrt(addUpForce * -2.0f * player.gravitY);

        force.y += upForce;

        player.TargetAddVelocity(player.connectionToClient, force);

        base.Call(player);
    }
}
