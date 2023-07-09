using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HurtTrigger : Hurtful
{
    [Header("StayInTrigger")]
    public float damagePerSeconds = 1.25f;
    private float timeSinceDamage = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox")
        {
            var hurtable = other.GetComponentInParent<Hurtable>(); //Hurtable should allways be parent
            if (hurtable != null)
            {
                HurtSomething(hurtable, damage, hurtType);
            }
        }
        else
        {
            var hurtable = other.GetComponent<Hurtable>();
            if (hurtable != null)
            {
                HurtSomething(hurtable, damage, hurtType);
            }
        }
    }
}
