using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemPickUp : NetworkBehaviour
{
    [Header("Move Propertys")]
    public float sinSpeed;
    public float sinHeight;

    public float rotSpeed = 90;

    private Vector3 orginPos;

    [Header("PickUp Propertys")]
    public bool isActive = true;
    public bool respawn = true;
    public float respawnDelay;
    private float respawnTime;

    //public enum PowerUp { None, Jump, Speed }

    [Header("Item Effects")]
    [Range(0,1)] public float healthPercentage; //relative to player max health
    public int special;
    public StatusEffect.EffectType effect = StatusEffect.EffectType.none;
    public float magnitude = 1;
    public float duration = 5;


    [Header("Internals")]
    private Renderer render;
    private Collider trigger;

    private void Start()
    {
        render = GetComponent<Renderer>();
        trigger = GetComponent<Collider>();

        orginPos = transform.localPosition;
        orginPos += Vector3.up * sinHeight * 1.5f;
    }

    void Update()
    {

        if (isServer && isActive == false)
        {
            respawnTime += Time.deltaTime;
            if (respawnTime > respawnDelay)
            {
                isActive = true;
                RpcSetItem(true);
            }
        }

        float yPos = orginPos.y + Mathf.Sin(Time.time * sinSpeed) * sinHeight;

        transform.localPosition = new Vector3(orginPos.x, yPos, orginPos.z);

        transform.localEulerAngles = new Vector3(0, Time.time * rotSpeed, 0);
    }

    [ClientRpc]
    public void RpcSetItemPosition(Vector3 position)
    {
        transform.position = position;
        transform.eulerAngles = Vector3.zero;

        orginPos = position;
        orginPos += Vector3.up * sinHeight * 1.5f;
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (isActive == false)
            return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            if (player.health <= 0)
                return;

            player.Hurt((int)(-player.maxHealth * healthPercentage)); //makes player gain health (WACKY)
            player.ServerAddSpecial(special);

            if (effect != StatusEffect.EffectType.none)
            {
                NetworkIdentity target = player.netIdentity;
                player.ApplyEffect(effect, duration, magnitude);
            }

            if (respawn)
            {
                respawnTime = 0;
                RpcSetItem(false);
            }

            else
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    [ClientRpc]
    void RpcSetItem(bool active)
    {
        render.enabled = active;
        trigger.enabled = active;

        isActive = active;
    }
}
