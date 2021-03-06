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
    public bool child = false;
    public float throwDelay = 0.125f;
    public float throwCoolDown = 0.4f;
    private float throwTime = 0;

    public float throwVelocity = 21;

    [Header("GroundPound Propertys")]
    public bool groundPound = true;
    public float chargeDuration = 2.75f;
    private float chargeTime;
    public float chargeSpeed = 5;
    public float minDistanceFromGround = 3;
    public float jumpHeight = 4;
    public float afterJumpDelay = 0.65f;

    public float specialChainCount = 5;


    private Vector3 chargeDirection;

    [Header("Weapon Refrences")]
    public GameObject punchHand;

    public Transform shootPos;
    public GameObject projectile; //some tf2 codeing level codeing here (doesnt do anything but might break else where if removed)

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
                player.CmdSpawnObject(2, player.transform.position - Vector3.up, Vector3.zero, false, false);
                EndSpecial();
            }

            else if (player.character.isGrounded && groundPound)
            {
                player.CmdSpawnObject(2, player.transform.position - Vector3.up, Vector3.zero, false, false);
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

        Invoke(nameof(Punch), punchDelay);
        timeSincePunch = 0;
    }

    void Punch()
    {
        player.CmdSpawnObject(0, Vector3.zero, transform.eulerAngles, false, true);
        player.velocity += transform.forward * punchVelocity;

        player.playerCam.currentOffset += punchRot;

        base.UsePrimary();
    }


    [Client]
    public override void UseSeconday()
    {
        if (player.paused)
            return;

        if (throwTime < throwCoolDown)
            return;

        base.UseSeconday();

        Invoke(nameof(Throw), throwDelay);
        throwTime = 0;
    }

    void Throw()
    {
        player.playerCam.currentOffset -= punchRot * 1.3f;

        if(child)
            player.CmdSpawnObject(1, shootPos.localPosition, shootPos.eulerAngles, false, true);
        else
            player.CmdShootObject(1, shootPos.position, shootPos.eulerAngles, false, true);

        player.velocity += shootPos.forward * throwVelocity;
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.paused)
            return; 

        //whould like base.useSpecial to run this but havent figured that out yet give me 7 years
        if (player.special - specialCost < 0) //not special enough falount 7 refrence (ADIAN HOLDSWORTH)
            return;

        player.CmdAddSpecial(-specialCost);


        if (groundPound)
        {
            if (player.DistanceFromGround() < minDistanceFromGround)
            {
                player.velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * player.gravitY * 2); //physics reasons
                Invoke(nameof(GroundPound), afterJumpDelay);
                return;
            }

            GroundPound();
        }

        else
        {
            ChainExsplotion();
        }
    }

    public void GroundPound()
    {
        specialIsActive = true;
        playerAnimator.animator.SetBool("Falling", true);

        player.velocity = Vector3.zero;

        float maxXZ = 0.6f;

        chargeDirection = new Vector3(Mathf.Clamp(transform.forward.x, -maxXZ, maxXZ),
            -1,
            Mathf.Clamp(transform.forward.z, -maxXZ, maxXZ)).normalized;

        chargeTime = 0;
    }

    public void ChainExsplotion()
    {
        for (int r = 0; r < specialChainCount; r++)
        {
            Vector3 randRot = new Vector3(Random.Range(-30, -120), Random.Range(-180, 180), Random.Range(-180, 180));
            player.CmdShootObject(1, shootPos.position, randRot, false, true);
        }

        //specialIsActive = true;

        //player.velocity = Vector3.zero;

        //float maxXZ = 0.6f;

        //chargeDirection = transform.forward;

        //chargeTime = 0;
    }

    public override void EndSpecial()
    {
        playerAnimator.animator.SetBool("Falling", false);

        //gotta reset the player
        //player.pushForce = orginalPushForce;
        //player.gravitY = orginalGravitY;
        player.velocity = player.velocity / 2;

        chargeTime = chargeDuration; //make sure if called outside of base

        base.EndSpecial();
    }
}
