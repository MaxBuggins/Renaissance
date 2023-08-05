using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurtfulSphere : Hurtful
{
    [Header("Sphere Collider")]
    public float radius = 1;
    public LayerMask mask;

    [Header("Internals")]
    private Vector3 lastPos;

    private void Update()
    {
        if (isServer)//Adian Smells of car fuel
        {
            //Debug.DrawLine(transform.position, lastPos, Color.green, 10);

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.position - lastPos,
                maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos) * 2f),
                mask, QueryTriggerInteraction.Ignore);
            /*
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 3)
                {
                    Player player = hit.collider.gameObject.GetComponent<Player>();
                    if (player != null)
                    {
                        if (hurtful.ignorOwner && player == hurtful.owner)
                            return; //dont interact with the shooter

                        float dist = Vector3.Distance(orginPos, transform.position);
                        int dmg = damage;

                        if (dist > startDmgFallOff) //Dmg roll off math
                        {
                            dist -= startDmgFallOff;
                            dmg = (int)(-0.0005f * (Mathf.Pow(dist, 4)) + dmg); //math moment with aidan
                            if (dmg < damage * minDmgMultiplyer) //min of 25% at about the distance of fog
                                dmg = (int)(damage * minDmgMultiplyer);
                        }

                        hurtful.HurtSomething(player, dmg, hurtful.hurtType);

                        if (bounceOffPlayerOnly)
                        {
                            destoryOnHits += 1;
                        }

                        else if (destoryOnPlayerHit)
                            DestroySelfHit();
                    }
                    else
                    {
                        PlayerHitBox playerHitBox = hit.collider.gameObject.GetComponent<PlayerHitBox>();

                        if (playerHitBox != null)
                        {
                            print("headShot");
                            if (hurtful.ignorOwner && playerHitBox.player == hurtful.owner)
                                return; //dont interact with the shooter

                            float dist = Vector3.Distance(orginPos, transform.position);
                            int dmg = (int)(damage * playerHitBox.damageMultiplyer);

                            if (dist > startDmgFallOff) //Dmg roll off math
                            {
                                dist -= startDmgFallOff;
                                dmg = (int)(-0.0005f * (Mathf.Pow(dist, 4)) + dmg); //math moment with aidan
                                if (dmg < damage * minDmgMultiplyer) //min of 25% at about the distance of fog
                                    dmg = (int)(damage * minDmgMultiplyer);
                            }

                            hurtful.HurtSomething(playerHitBox.player, dmg, hurtful.hurtType);

                            if (bounceOffPlayerOnly)
                            {
                                destoryOnHits += 1;
                            }

                            else if (destoryOnPlayerHit)
                                DestroySelfHit();
                        }
                    }
                }
                else
                {
                    if (hitSplat != null)
                        Instantiate(hitSplat, hit.point, Quaternion.LookRotation(hit.normal));
                }

                if (mustHitNormalGround == true && Vector3.Dot(hit.normal, Vector2.up) < 0.9f) //Dot your nannny
                    destoryOnHits++; //allows the projectile to be hit again 

                print(hit.normal);

                if (destoryOnHits > -1)
                {
                    destoryOnHits -= 1;

                    if (destoryOnHits < 0)
                        DestroySelfHit();
                    else
                    {
                        Vector3 forw = (transform.position - lastPos).normalized;
                        Vector3 mirrored = Vector3.Reflect(forw, hit.normal);
                        transform.rotation = Quaternion.LookRotation(mirrored, transform.up);
                        transform.position = hit.point;
                        velocity = Vector3.zero; //reset for gravity projectiles
                        totalAirRistance = 0;

                        if (hurtful != null)
                            hurtful.ignorOwner = false;

                        RpcSyncProjectile(transform.position, transform.eulerAngles, !bounceOffPlayerOnly);
                    }
                }
            */
        }
            
    }

}
