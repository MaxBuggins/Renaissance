using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoxerWeapon : PlayerWeapon
{
    [Header("Gun Propertys")]
    public float throwDelay = 0.125f;
    private float throwTime = 0;

    [Header("Weapon Refrences")]
    public Transform shootPos;
    public GameObject projectile;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();

        base.Start();
    }

    protected override void Update()
    {
        throwTime += Time.deltaTime;
        base.Update();
    }

    [Client]
    public override void UsePrimary()
    {

        base.UsePrimary();
    }

    [Client]
    public override void UseSeconday()
    {
        if (throwTime < throwDelay)
            return;

        throwTime = 0;

        player.CmdSpawnObject(0, shootPos.position, shootPos.eulerAngles);

        base.UseSeconday();
    }

    [Client]
    public override void UseSpecial()
    {
        player.speed = 25;
    }
}
