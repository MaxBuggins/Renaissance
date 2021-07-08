using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoxerWeapon : PlayerWeapon
{
    [Header("Throw Propertys")]
    public float throwDelay = 0.125f;
    private float throwTime = 0;

    [Header("Charge Propertys")]
    public float chargeDuration = 2.75f;
    private float chargeTime;
    public float chargeSpeed = 5;
    public float gravityMultiplyer = 0.5f;

    private Vector3 chargeDirection;

    [Header("Weapon Refrences")]
    public Transform shootPos;
    public GameObject projectile;
    public GameObject hurtCube;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();
        chargeTime = chargeDuration;

        base.Start();
    }

    protected override void Update()
    {
        throwTime += Time.deltaTime;

        if (chargeTime < chargeDuration)
        {
            chargeTime += Time.deltaTime;
            player.character.Move(chargeDirection * chargeSpeed * Time.deltaTime);
            if (chargeTime >= chargeDuration)
                EndSpecial();
        }

        base.Update();
    }

    [Client]
    public override void UsePrimary()
    {
        player.CmdSpawnObject(0, transform.position, transform.eulerAngles, true);
        base.UsePrimary();
    }

    [Client]
    public override void UseSeconday()
    {
        if (throwTime < throwDelay)
            return;

        throwTime = 0;

        player.CmdSpawnObject(1, shootPos.position, shootPos.eulerAngles, false);

        base.UseSeconday();
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.special - specialCost < 0)
            return;

        player.CmdUseSpecial(specialCost);

        player.pushForce *= 7;
        player.gravitY *= gravityMultiplyer;
        chargeDirection = transform.forward;
        chargeTime = 0;
    }

    void EndSpecial()
    {
        player.pushForce /= 7;
        player.gravitY /= gravityMultiplyer;
    }
}
