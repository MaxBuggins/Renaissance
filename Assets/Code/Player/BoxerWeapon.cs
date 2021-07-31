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

    public float punchVelocity = 6;

    [Header("Throw Propertys")]
    public float throwDelay = 0.125f;
    public float throwCoolDown = 0.4f;
    private float throwTime = 0;

    public float throwVelocity = 21;

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
        if (player.paused)
            return;

        throwTime += Time.deltaTime;
        timeSincePunch += Time.deltaTime;

        if (specialIsActive)
        {
            chargeTime += Time.deltaTime;
            player.velocity += chargeDirection * chargeSpeed * Time.deltaTime;
            //player.character.Move(chargeDirection * chargeSpeed * Time.deltaTime);


            if (chargeTime >= chargeDuration)
            {
                player.CmdSpawnObject(3, transform.position - Vector3.up, Vector3.zero, false, false);
                EndSpecial();
            }
        }

        base.Update();
    }

    [Client]
    public override void UsePrimary()
    {
        if (player.paused)
            return;

        if (timeSincePunch < punchCooldown)
            return;

        punchHand.transform.position += transform.forward * 0.75f;
        Invoke(nameof(Punch), punchDelay);
        timeSincePunch = 0;
    }

    void Punch()
    {
        player.CmdSpawnObject(0, Vector3.zero, transform.eulerAngles, true, true);
        player.velocity += transform.forward * punchVelocity;
        base.UsePrimary();
        punchHand.transform.position -= transform.forward * 0.75f;
    }


    [Client]
    public override void UseSeconday()
    {
        if (player.paused)
            return;

        if (throwTime < throwCoolDown)
            return;

        Invoke(nameof(Throw), throwDelay);
        throwTime = 0;
    }

    void Throw()
    {
        player.CmdSpawnObject(1, shootPos.position, shootPos.eulerAngles, false, false);
        player.velocity += shootPos.forward * throwVelocity;
        base.UseSeconday();
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.paused)
            return;

        //whould like base.useSpecial to run this but havent figured that out yet give me 7 years
        if (player.special - specialCost < 0) //not special enough falount 7 refrence (ADIAN HOLDSWORTH)
            return;

        base.UseSpecial();

        player.velocity = Vector3.zero;

        //orginalPushForce = player.pushForce;
        player.pushForce *= pushForceMultipyer;

        //orginalGravitY = player.gravitY;
        player.gravitY *= gravityMultiplyer;

        chargeDirection = -Vector3.up;  //new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        chargeTime = 0;

        player.CmdSpawnObject(2, -Vector3.up, Vector3.zero, false, true);
    }

    public override void EndSpecial()
    {
        //gotta reset the player
        player.pushForce = orginalPushForce;
        player.gravitY = orginalGravitY;
        player.velocity = player.velocity / 2;

        chargeTime = chargeDuration; //make sure if called outside of base

        base.EndSpecial();
    }
}
