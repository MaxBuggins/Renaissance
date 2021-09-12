using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OnIdle : NetworkBehaviour
{
    private Player player;

    public float spawnDelay = 2;

    public float idleVelocity = 0.4f;
    private bool idle;

    public float effectDelay = 0.3f;
    public HurtType hurtType = HurtType.Death;
    public float minSpecial = 0.2f;

    //public int idleSpecial = 2;
    //public int moveSpecial = -1;

    private float timeSinceMovement = 0;
    private float timeSinceIdle = 0;

    void Start()
    {
        if (!isServer)
            enabled = false;

        player = GetComponent<Player>();
    }

    void Update()
    {

        if (player.health == 0)
        {
            spawnDelay = 2;
            return;
        }

        if(spawnDelay > 0)
        {
            spawnDelay -= Time.deltaTime;
            return;
        }


        timeSinceMovement += Time.deltaTime;

        Vector3 velocity = (transform.position - player.lastPos);
        velocity.y = 0; //falling and jumping dont count
        idle = velocity.magnitude < idleVelocity; //magnitude is allways posative thanks aids


        if (timeSinceMovement > effectDelay)
        {
            float special = velocity.magnitude * player.playerClass.specialChargeRate;
            special += minSpecial;
            player.specialChargeRate = special;

            timeSinceMovement = 0;
        }
        /*

        else
            ResetTimer();

        if (timeSinceMovement > effectDelay)
        {

            if (player.special == player.playerClass.maxSpecial)
            {
                player.Hurt(damagePer * 2, hurtType);
            }
            else
            {
                player.Hurt(damagePer, hurtType);
                player.AddSpecial(idleSpecial);
            }
            ResetTimer();
        }
        */

            //else if (timeSinceIdle > effectDelay)
            //{
            //player.AddSpecial(moveSpecial);

            //if(player.special == 0)
            //{
            //    player.Hurt(damagePer);
            //}
            //ResetTimer();
            //}
    }

    void ResetTimer()
    {
        timeSinceIdle = 0;
        timeSinceMovement = 0;
    }

    private void Enable()
    {
        enabled = true;
    }
}
