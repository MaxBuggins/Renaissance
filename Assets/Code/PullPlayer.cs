using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPlayer : OnCall
{
    public float pullForce;

    [Header("Modifyers")]
    public float rootAmount = 2.5f;
    public float upMultiplyer = 1;
    public float addUpForce = 1;


    public override void Call(Player player)
    {

        Vector3 force = (transform.position - player.transform.position) * pullForce;

        if (force.y > 0)
        {
            force.y = Mathf.Pow(force.y, 1f / rootAmount);
            force.y *= upMultiplyer;

            force += Vector3.up * addUpForce;
        }

        player.TargetSetVelocity(player.connectionToClient, force);

        base.Call(player);
    }
}
