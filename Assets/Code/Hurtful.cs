using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum HurtType { Death, Water, Train, Punch, ShotPut, DeflectingBullet, BriefCase, ShockWave, Squash, Freeze, Plate, Gambling, Time }


//Server only script
public class Hurtful : NetworkBehaviour
{
    [Header("Hurt")]
    public int damage = 1;
    public HurtType hurtType = HurtType.Death;
    public bool destoryOnHurt = false;

    [Header("StatusEffect")]
    public StatusEffect.EffectType statusEffect;
    public float statusMagnitude;
    public float statusDuration;

    [Header("HurtRequirements")]
    public bool ignorOwner = true;

    [Header("Force")]
    public bool moveForce = true; //if false then force is caculated via distance from collider center
    public float collisionForce = 0;
    public float upwardsForce = 0;
    public float maxVelocity = -1;
    
    [HideInInspector] public Hurtable owner; //who takes the credit for a hit
    [HideInInspector][SyncVar] public uint ownerID;

    //[Header("RigidBodys")]
    //public bool destoryRigidbodys = false;
    //public float destroyDelay;

    private Vector3 lastPos;

    [Header("Unity Stuff")] //this isn't how I would do it but ummm okay sweaty :n) <-- Usopp from the famous film One Peace
    private Collider myCollider;

    void Start()
    {
        //this for some reason breaks hurt over time areas?
        //if (!isServer) //only for the server to run
            //enabled = false;

        if (moveForce)
        {
            collisionForce /= 100;
            upwardsForce /= 100;
        }

        myCollider = GetComponent<Collider>();

        NetworkIdentity ownerIdenity;

        if (owner == null) //works
        {
            //netIdentity.netId
            //NetworkIdentity.
                //.TryGetValue(ownerID, out ownerIdenity);
            //if(ownerIdenity != null)
                //owner = ownerIdenity.gameObject.GetComponent<Player>();
        }

        lastPos = transform.position;
    }

    [Server]
    private void Update()
    {     
        lastPos = transform.position;
    }


    [Server]
    public void HurtSomething(Hurtable hurtable, int damage, HurtType type)
    {
        print(name + " does " + damage + " to " + hurtable.name);

        if (damage == 0)
            return;

        if (hurtable == owner && ignorOwner)
            return;

        if (collisionForce != 0) //whats the point if its 0
        {
            Vector3 vel;
            if (moveForce || myCollider == null) //its a fix i guess
                vel = (transform.position - lastPos) / Time.deltaTime;
            else
                vel = (hurtable.transform.position - myCollider.bounds.center).normalized * 3;

            if (maxVelocity > 0)
                vel = Vector3.ClampMagnitude(vel, maxVelocity);

            Player player = hurtable.gameObject.GetComponent<Player>();
            if (player != null)
                player.TargetAddVelocity(hurtable.connectionToClient, (vel * collisionForce) + (vel.magnitude * Vector3.up * upwardsForce));
        }

        if (statusMagnitude > 0)
            hurtable.ServerApplyEffect(statusEffect, statusDuration, statusMagnitude);

        if (owner != null)
        {
            hurtable.Hurt(damage, type, netIdentity, owner.netIdentity);
        }
        else
            hurtable.Hurt(damage, type, netIdentity);

        if (hurtable.health <= 0)
        {          
            if (owner != null && owner != hurtable) //on owner acturlly killing something (Like a Boss Baby)
            {
                //owner.playerStats.killStreak += 1;
                //owner.playerStats.kills += 1;
            }
        }

        if (destoryOnHurt == true)
            Destroy(gameObject);
    }
}
