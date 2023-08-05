using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class HurtfulMelee : Hurtful
{
    [Header("Cast Domentions")]
    public float capsualeLength;
    public float capsualeRadius;
    public Vector3 capsualeDirection;


    void Start()
    {
        Physics.CapsuleCast(transform.position, transform.position + transform.forward * capsualeLength, capsualeRadius, capsualeDirection);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, capsualeRadius);
        Gizmos.DrawWireSphere(transform.position + transform.forward * capsualeLength, capsualeRadius);
    }

}
