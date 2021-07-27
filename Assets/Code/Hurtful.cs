using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum HurtType { Death, Water, Train, Punch, ShotPut }

//Server only script
public class Hurtful : NetworkBehaviour
{
    [Header("Hurt")]
    public int damage = 1;
    public HurtType hurtType = HurtType.Death;
    public bool destoryOnHurt = false;

    public float damagePerSeconds = 1.25f;
    private float timeSinceDamage = 0;

    public bool moveForce = true; //if false then force is caculated via distance from collider center
    public float collisionForce = 0;
    public float upwardsForce = 0;

    private List<Player> inTrigger = new List<Player>();

    public Player owner; //who takes the credit for a hit
    [SyncVar] public uint ownerID;

    public bool ignorOwner = true;

    [Header("RigidBodys")]
    public bool destoryRigidbodys = false;
    public float destroyDelay;

    private Vector3 lastPos;

    [Header("Unity Stuff")] //this isn't how I would do it but ummm okay sweaty :n) <-- Usopp from the famous film One Peace
    private Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();

        NetworkIdentity ownerIdenity;

        if (owner == null) //works
        {
            NetworkIdentity.spawned.TryGetValue(ownerID, out ownerIdenity);
            if(ownerIdenity != null)
                owner = ownerIdenity.gameObject.GetComponent<Player>();
        }
        


        if (!isServer) //only for the server to run
            enabled = false;

        lastPos = transform.position;
    }

    private void Update()
    {
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                if (ignorOwner == false || player != owner)
                {
                    HurtPlayer(player, damage, hurtType);
                    inTrigger.Add(player);

                    if (owner != null)
                        owner.ConfirmedHit(player.health <= 0);
                }
            }
            return;
        }

        if (!destoryRigidbodys)
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
            StartCoroutine(WaitThenDestory(rb.gameObject));
    }

    [Server]
    private void OnTriggerExit(Collider other)
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
    IEnumerator WaitThenDestory(GameObject obj)
    {
        yield return new WaitForSeconds(destroyDelay);

        if(obj != null)
            if (obj.GetComponent<NetworkIdentity>() != null)
                NetworkServer.Destroy(obj);
    }

    [Server]
    public void HurtPlayer(Player player, int damage, HurtType type)
    {
        if (player == owner && ignorOwner)
            return;

        if(owner != null)
            player.Hurt(damage, type, owner.playerName);
        else
            player.Hurt(damage, type, "");

        if (player.health <= 0)
        {
            inTrigger.Remove(player);
            if (owner != null && owner != player) //on owner acturlly killing something (Like a Boss Baby)
            {
                owner.killStreak += 1;
                owner.score += 2;
            }
        }

        if(collisionForce != 0)
        {
            Vector3 vel;
            if (moveForce)
                vel = transform.position - lastPos;
            else
                vel = player.transform.position - collider.bounds.center;

            player.RpcAddVelocity((vel * collisionForce) + (vel.magnitude * Vector3.up * upwardsForce));
        }


        if (destoryOnHurt == true)
            Destroy(gameObject);
    }
}
