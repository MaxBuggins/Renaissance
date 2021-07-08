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

    [Header("Item Propertys")]
    public bool isActive = true;
    public float respawnDelay;
    private float respawnTime;

    public int health;
    public int special;

    [Header("Internals")]
    private Renderer render;

    private void Start()
    {
        render = GetComponent<Renderer>();

        orginPos = transform.position;
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

        transform.position = new Vector3(orginPos.x, yPos, orginPos.z);

        transform.eulerAngles = new Vector3(0, Time.time * rotSpeed, 0);
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (isActive == false)
            return;

        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.health += health;
            player.special += special;

            isActive = false;
            respawnTime = 0;
            RpcSetItem(false);
        }
    }

    [ClientRpc]
    void RpcSetItem(bool active)
    {
        render.enabled = active;
    }
}
