using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FireFighterWeapon : PlayerWeapon
{
    [Header("Punch Propertys")]
    public float punchDelay;
    public float punchCooldown;
    private float timeSincePunch;

    //public float punchVelocity = 6;

    public Vector3 swingRot;

    [Header("Shoot Propertys")]
    public float fireRate = 0.4f;
    private float fireTime = 0;

    public int clipSize = 6;
    private int clipAmount;

    private bool reloading = false;
    public float reloadRate = 2;
    private float reloadTime = 0;

    [Header("FireHydrant Placemnt")]
    public float jumpForce = 10;
    public float plantDelay = 0.25f;
    public float plantOffsetY = -1;

    public LayerMask summonMask;
    public float maxDistance = 5;

    public ClientActivated fireHydrant;

    [Header("Weapon Refrences")]


    private Animator animator;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();

        clipAmount = clipSize;

        base.Start();
    }

    protected override void Update()
    {
        if (player.paused)
            return;

        base.Update();
    }

    [Client]
    public override void UseSeconday()
    {
        //player.velocity += Vector3.up * jumpForce;

        //Invoke(nameof(Plant), plantDelay);

        base.UseSeconday();
    }

    void Plant()
    {
        Vector3 spawnTrans = player.transform.position + Vector3.up * plantOffsetY;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray: ray, maxDistance: maxDistance, hitInfo: out hit, layerMask: summonMask))
        {
            spawnTrans = hit.point;
        }

        Vector3 spawnRot = player.transform.eulerAngles;
        player.CmdSpawnObject(0, spawnTrans, spawnRot, false, false);
    }


    [Client]
    public override void UsePrimary()
    {
        if (player.paused || reloading)
            return;

        base.UsePrimary();
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.paused)
            return;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray: ray, maxDistance: 2, hitInfo: out hit, layerMask: summonMask))
        {
            if (hit.transform.gameObject == fireHydrant.gameObject)
            {
                NetworkServer.Destroy(fireHydrant.gameObject);
                fireHydrant = null;

                base.UseSpecial();

                return;
            }
        }

        if (fireHydrant == null)
        {
            player.velocity += Vector3.up * jumpForce;
            Invoke(nameof(Plant), plantDelay);
        }
        else
        {
            SlamHydrant();
        }

        base.UseSpecial();
    }

    public void SlamHydrant()
    {
        fireHydrant.netIdentity.AssignClientAuthority(netIdentity.connectionToClient);
        fireHydrant.CmdEvent();
    }
}