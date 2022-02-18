using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OnIdle : NetworkBehaviour
{
    private Player player;

    public float spawnDelay = 1.5f;
    private float timeSinceSpawn = 0;

    //public float maxIdleMagnitude = 0.25f;

    public float idleTimeForSpecial = 0.9f;
    private float timeSinceIdle;




    //public float idleVelocity = 0.4f;

    //public float effectDelay = 0.3f;
    //public HurtType hurtType = HurtType.Death;
    //public float minSpecial = 0.2f;

    //public int moveSpecial = -1;

    //private float timeSinceMovement = 0;
    //private float timeSinceIdle = 0;

    private Vector3 lastPos;

    void Start()
    {
        if (!isServer)
            enabled = false;

        player = GetComponent<Player>();
    }

    [ClientCallback] //VERY hackable but whos gonna hack this thing anyway
    void Update()
    {
        if (player.health == 0)
        {
            timeSinceSpawn = 0;
            return;
        }

        if(timeSinceSpawn < spawnDelay)
        {
            timeSinceSpawn += Time.deltaTime;
            return;
        }

        if(player.move == Vector2.zero)
        {
            timeSinceIdle += Time.deltaTime;

            if (timeSinceIdle > idleTimeForSpecial)
            {
                player.CmdAddSpecial(1);
                timeSinceIdle = 0;
            }
        }
        else
            timeSinceIdle = 0;

        //float velMag = (transform.position - lastPos).magnitude;

        //if(velMag <= maxIdleMagnitude)


        //lastPos = transform.position;

        /*        velocity.y = 0; //falling and jumping dont count
                idle = velocity.magnitude < idleVelocity; //magnitude is allways posative thanks aids


                if (timeSinceMovement > effectDelay)
                {
                    float special = velocity.magnitude * player.playerClass.specialChargeRate;
                    special += minSpecial;
                    player.specialChargeRate = special;

                    timeSinceMovement = 0;

                    //if (player.special >= player.maxSpecial)
                    //player.Hurt(5);
                }*/
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

    //void ResetTimer()
    //{
        //timeSinceIdle = 0;
        //timeSinceMovement = 0;
    //}

    //private void Enable()
    //{
        //enabled = true;
    //}
}
