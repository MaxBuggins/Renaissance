using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//this script is for the local client to run only
public class Gun : PlayerWeapon
{
    [Header("Gun Propertys")]
    public float shootDelay = 0.125f;
    private float shootTime = 0;

    public int clipSize = 6;
    private int clipLeft; 

    public float reloadDelay = 1.2f;
    private float reloadTime = 0;

    [Header("Gun Refrences")]
    public Transform shootPos;
    public GameObject projectile;

    private Player player;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();
        clipLeft = clipSize;

        base.Start();
    }

    protected override void Update()
    {
        shootTime += Time.deltaTime;
        base.Update();
    }
    
    [Client]
    public override void UsePrimary()
    {
        if (shootTime < shootDelay)
            return;

        shootTime = 0;

        player.CmdSpawnObject(0, shootPos.position, shootPos.eulerAngles, false);

        base.UsePrimary();
    }
}
