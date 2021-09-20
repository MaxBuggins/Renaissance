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

    public Vector3 punchRot;

    [Header("Throw Propertys")]
    public float throwDelay = 0.125f;
    public float throwCoolDown = 0.4f;
    private float throwTime = 0;

    public float throwVelocity = 21;

    [Header("GroundPound Propertys")]
    public float chargeDuration = 2.75f;
    private float chargeTime;
    public float chargeSpeed = 5;
    public float minDistanceFromGround = 3;
    public float jumpHeight = 4;
    public float afterJumpDelay = 0.65f;


    private Vector3 chargeDirection;

    [Header("Weapon Refrences")]
    public GameObject punchHand;

    public Transform shootPos;
    public GameObject projectile;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();
        chargeTime = chargeDuration;

        //orginalPushForce = player.pushForce;
        //orginalGravitY = player.gravitY;

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
                player.CmdSpawnObject(3, player.transform.position - Vector3.up, Vector3.zero, false, false);
                EndSpecial();
            }

            else if (player.character.isGrounded)
            {
                player.CmdSpawnObject(3, player.transform.position - Vector3.up, Vector3.zero, false, false);
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



        player.playerCam.currentOffset += punchRot;

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
        player.playerCam.currentOffset -= punchRot * 1.3f;

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

        if (player.DistanceFromGround() < minDistanceFromGround)
        {
            player.velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * player.gravitY * 2); //physics reasons
            Invoke(nameof(GroundPound), afterJumpDelay);
            return;
        }

        GroundPound();
    }

    public void GroundPound()
    {
        base.UseSpecial();

        player.velocity = Vector3.zero;

        float maxXZ = 0.6f;

        chargeDirection = new Vector3(Mathf.Clamp(transform.forward.x, -maxXZ, maxXZ),
            -1,
            Mathf.Clamp(transform.forward.z, -maxXZ, maxXZ)).normalized;

        chargeTime = 0;
    }

    public override void EndSpecial()
    {
        //gotta reset the player
        //player.pushForce = orginalPushForce;
        //player.gravitY = orginalGravitY;
        player.velocity = player.velocity / 2;

        chargeTime = chargeDuration; //make sure if called outside of base

        base.EndSpecial();
    }
}
