using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Server only script
public class Hurtful : NetworkBehaviour
{
    [Header("Players")]
    public int damage = 1;
    public bool destoryOnHurt = false;

    private float timeSinceDamage = 0;

    public float collisionForce = 0;
    public float upwardsForce = 0;

    private List<Player> inTrigger = new List<Player>();

    public Player ignorePlayer;

    [Header("RigidBodys")]
    public bool destoryRigidbodys = false;
    public float destroyDelay;

    private Vector3 lastPos;


    void Awake()
    {
        if (isClient) //only for the server to run
            enabled = false;

        lastPos = transform.position;
    }

    private void Update()
    {
        if (inTrigger.Count == 0)
            return;

            timeSinceDamage += Time.deltaTime;

        if(timeSinceDamage > 1)
        {
            //foreach throws errors not sure why
            for(int i = 0; i < inTrigger.Count; i++)
            {
                HurtPlayer(inTrigger[i], damage);
            }

            timeSinceDamage = 0;
        }

        lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var player = other.GetComponent<Player>();
            if (player != null && player != ignorePlayer)
            {
                HurtPlayer(player, damage);

                if (destoryOnHurt == true)
                    NetworkServer.Destroy(gameObject);

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

    IEnumerator WaitThenDestory(GameObject obj)
    {
        yield return new WaitForSeconds(destroyDelay);
        NetworkServer.Destroy(obj);
    }

    [Server]
    void HurtPlayer(Player player, int Damage)
    {
        player.health -= damage;
        if(player.health <= 0)
        {
            inTrigger.Remove(player);
        }

        if(collisionForce != 0)
        {
            Vector3 vel = transform.position - lastPos;
            player.RpcAddVelocity((vel * collisionForce) + (vel.magnitude * Vector3.up * upwardsForce));
        }
    }
}
