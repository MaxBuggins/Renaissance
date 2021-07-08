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

    private List<Player> inTrigger = new List<Player>();

    public Player ignorePlayer;

    [Header("RigidBodys")]
    public bool destoryRigidbodys = false;
    public float destroyDelay;


    void Awake()
    {
        if (isClient) //only for the server to run
            enabled = false;
    }

    private void Update()
    {
        timeSinceDamage += Time.deltaTime;

        if(timeSinceDamage > 1)
        {
            foreach (Player player in inTrigger)
                HurtPlayer(player, damage);

            timeSinceDamage = 0;
        }
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

    void HurtPlayer(Player player, int Damage)
    {
        player.health -= damage;
        if(player.health <= 0)
        {
            inTrigger.Remove(player);
        }
    }
}
