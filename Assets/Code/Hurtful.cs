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

    public Player ignorePlayer;

    [Header("RigidBodys")]
    public bool destoryRigidbodys = false;
    public float destroyDelay;

    private Vector3 lastPos;

    public Sprite hurtSprite;

    [Header("Unity Stuff")]
    private Collider collider;

    void Awake()
    {
        collider = GetComponent<Collider>();

        if (isClient) //only for the server to run
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
            if (player != null && player != ignorePlayer)
            {
                if (ignorePlayer != false)
                {
                    if(isClient)
                        ignorePlayer.ConfirmedHit(ignorePlayer.connectionToServer);
                    else
                        ignorePlayer.ConfirmedHit(ignorePlayer.connectionToClient);
                }

                HurtPlayer(player, damage, hurtType);

                inTrigger.Add(player);
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
        if (player == ignorePlayer)
            return;

        if(ignorePlayer != null)
            player.Hurt(damage, type, ignorePlayer.playerName);
        else
            player.Hurt(damage, type, "");

        if (player.health <= 0)
        {
            inTrigger.Remove(player);
            if (ignorePlayer != null)
                ignorePlayer.score += 1;
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
