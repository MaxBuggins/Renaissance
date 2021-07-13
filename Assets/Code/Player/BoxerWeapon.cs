using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoxerWeapon : PlayerWeapon
{
    [Header("Punch Propertys")]
    public float punchDelay;
    public float punchCooldown;
    private float timeSincePunch;

    [Header("Throw Propertys")]
    public float throwDelay = 0.125f;
    private float throwTime = 0;

    [Header("Charge Propertys")]
    public float chargeDuration = 2.75f;
    private float chargeTime;
    public float chargeSpeed = 5;

    public float pushForceMultipyer = 5;
    public float gravityMultiplyer = 0.5f;

    //to store default values
    private float orginalPushForce;
    private float orginalGravitY;

    private Vector3 chargeDirection;

    [Header("Weapon Refrences")]
    public GameObject punchHand;

    public Transform shootPos;
    public GameObject projectile;
    public GameObject hurtCube;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();
        chargeTime = chargeDuration;

        orginalPushForce = player.pushForce;
        orginalGravitY = player.gravitY;

        base.Start();
    }

    protected override void Update()
    {
        throwTime += Time.deltaTime;
        timeSincePunch += Time.deltaTime;

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
        if (timeSincePunch < punchCooldown)
            return;

        punchHand.transform.position += transform.forward * 0.75f;
        Invoke(nameof(Punch), punchDelay);
        timeSincePunch = 0;
    }

    void Punch()
    {
        player.CmdSpawnObject(0, player.transform.position, transform.eulerAngles, true, true);
        base.UsePrimary();
        punchHand.transform.position -= transform.forward * 0.75f;
    }


    [Client]
    public override void UseSeconday()
    {
        if (throwTime < throwDelay)
            return;

        throwTime = 0;

        player.CmdSpawnObject(1, shootPos.position, shootPos.eulerAngles, false, false);

        base.UseSeconday();
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.special - specialCost < 0)
            return;

        player.CmdUseSpecial(specialCost);

        player.velocity = Vector3.zero;

        //orginalPushForce = player.pushForce;
        player.pushForce *= pushForceMultipyer;

        //orginalGravitY = player.gravitY;
        player.gravitY *= gravityMultiplyer;

        chargeDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        chargeTime = 0;

        player.CmdSpawnObject(2, transform.position, transform.eulerAngles, true, true);
    }

    public override void EndSpecial()
    {
        player.pushForce = orginalPushForce;
        player.gravitY = orginalGravitY;

        chargeTime = chargeDuration; //make sure if called outside of script
    }
}
