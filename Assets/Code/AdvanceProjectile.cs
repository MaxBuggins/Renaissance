using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class AdvanceProjectile : Projectile
{
    [Header("Rotation")]
    public float startDistance = 0;

    public Vector3 rotaionAmount;
    public float duration;
    public float delay;
    public AnimationCurve rotCurve;


    public bool trackPlayer;
    public float targetSpeed = 5;

    public LayerMask playerMask;
    private Transform target;


    private void Start()
    {


        //Tween.Rotate(transform, rotaionAmount, Space.Self, duration, delay, rotCurve);
        base.Awake();
    }

    private void lateStart()
    {
        if (rotaionAmount != Vector3.zero)
        {
            Tween.Rotate(transform, rotaionAmount, Space.Self, duration, delay, rotCurve);
            startDistance = Mathf.Infinity; //never again
        }

        if (trackPlayer)
        {
            Player[] players = FindObjectsOfType<Player>();

            if (players.Length < 2)
            {
                return;
            }

            foreach (Player player in players)
            {
                if(target == null)
                {
                    if (player != hurtful.owner)
                        target = player.transform;
                }

                else if (Vector3.Distance(player.transform.position, transform.position) < Vector3.Distance(target.position, transform.position))
                {
                    if(player != hurtful.owner)
                        target = player.transform;
                }
            }
        }


    }

    private void Update()
    {
        if (Vector3.Distance(orginPos, transform.position) > startDistance)
        {
            lateStart();
        }

        if (target != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, targetSpeed * Time.deltaTime);
        }

        base.Update();
    }
}
