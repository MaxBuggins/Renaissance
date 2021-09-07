using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum HurtType { Death, Water, Train, Punch, ShotPut, DeflectingBullet, BriefCase, ShockWave, Squash, Freeze}


//Server only script
public class Hurtful : NetworkBehaviour
{
    [Header("Hurt")]
    public int damage = 1;
    public HurtType hurtType = HurtType.Death;
    public StatusEffect.EffectType statusEffect;
    public float statusMagnitude;
    public float statusDuration;
    public bool destoryOnHurt = false;

    public float damagePerSeconds = 1.25f;
    private float timeSinceDamage = 0;

    public bool ignorOwner = true;

    [Header("Force")]
    public bool moveForce = true; //if false then force is caculated via distance from collider center
    public float collisionForce = 0;
    public float upwardsForce = 0;
    public float maxVelocity = -1;

    private List<Player> inTrigger = new List<Player>();

    public Player owner; //who takes the credit for a hit
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
            NetworkIdentity.spawned.TryGetValue(ownerID, out ownerIdenity);
            if(ownerIdenity != null)
                owner = ownerIdenity.gameObject.GetComponent<Player>();
        }

        lastPos = transform.position;
    }

    [Server]
    private void Update()
    {
        //print(inTrigger.Count);
        if (inTrigger.Count == 0)
            return;

        timeSinceDamage += Time.deltaTime;

        if(timeSinceDamage > damagePerSeconds && damagePerSeconds > 0)
        {
            //foreach throws errors not sure why
            for(int i = 0; i < inTrigger.Count; i++)
            {
            HurtPlayer(inTrigger[i], damage, hurtType);
            }

            timeSinceDamage = 0;
        }

        lastPos = transform.position;
    }

    [Server]
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                if (ignorOwner == false || player != owner)
                {
                    //if(damagePerSeconds <= 0)
                        HurtPlayer(player, damage, hurtType);
                    inTrigger.Add(player);

                    if (owner != null)
                        owner.ConfirmedHit(player.health <= 0);
                }
            }
            return;
        }
    }

    [Server]
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                inTrigger.Remove(player);
            }
        }
    }

    [Server]
    public void HurtPlayer(Player player, int damage, HurtType type)
    {
        if (damage == 0)
            return;

        if (player == owner && ignorOwner)
            return;

        if (collisionForce != 0)
        {
            Vector3 vel;
            if (moveForce || myCollider == null) //its a fix i guess
                vel = (transform.position - lastPos) / Time.deltaTime;
            else
                vel = (player.transform.position - transform.position).normalized * 3;

            if (maxVelocity > 0)
                vel = Vector3.ClampMagnitude(vel, maxVelocity);

            player.TargetAddVelocity(player.connectionToClient, (vel * collisionForce) + (vel.magnitude * Vector3.up * upwardsForce));
        }

        if (owner != null)
        {
            player.Hurt(damage, type, owner.playerName);
        }
        else
            player.Hurt(damage, type, "");

        if (statusMagnitude > 0)
            player.applyEffect(statusEffect, statusDuration, statusMagnitude);

        if (player.health <= 0)
        {
            inTrigger.Remove(player);
            if (owner != null && owner != player) //on owner acturlly killing something (Like a Boss Baby)
            {
                owner.killStreak += 1;
                owner.score += 2;
            }
        }

        if (destoryOnHurt == true)
            Destroy(gameObject);
    }
}
