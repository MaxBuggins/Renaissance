using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MaidWeapon : PlayerWeapon
{
    [Header("Punch Propertys")]
    public float punchDelay;
    public float punchCooldown;
    private float timeSincePunch;

    public float punchVelocity = 6;

    [Header("KnifeThrow Propertys")]
    public float fireRate = 0.25f;
    private float fireTime = 0;

    public int clipSize = 2;
    private int clipAmount;

    private bool reloading = false;
    public float reloadRate = 0.5f;
    private float reloadTime = 0;

    public Vector3 throwRot;

    [Header("BriefCaseSmash Propertys")]
    public Vector3 summonRot;


    [Header("Weapon Refrences")]
    public GameObject punchHand;
    public GameObject iceBlock;

    public Transform shootPos;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();

        clipAmount = clipSize;

        base.Start();
    }

    protected override void Update()
    {
        if (player.paused)
            return;

        fireTime += Time.deltaTime;

        reloadPerstenage = reloadTime / reloadRate;

        if (reloading)
        {
            reloadTime += Time.deltaTime;

            if (reloadTime > reloadRate)
            {
                clipAmount = clipSize;
                reloading = false;
                reloadTime = 0;
            }
        }

        timeSincePunch += Time.deltaTime;

        base.Update();
    }

    [Client]
    public override void UsePrimary()
    {
        if (player.paused || reloading)
            return;

        if (fireTime < fireRate)
            return;

        if (clipAmount <= 0)
        {
            reloading = true;
            return;
        }

        clipAmount -= 1;

        player.CmdSpawnObject(1, shootPos.position, shootPos.eulerAngles, false, false);
        base.UsePrimary();
        fireRate = 0;

        if (clipAmount <= 0)
            reloading = true;
    }

    void Punch()
    {
        player.playerCam.currentOffset -= throwRot;

        player.CmdSpawnObject(0, Vector3.zero, transform.eulerAngles, true, true);
        player.velocity += transform.forward * punchVelocity;
        base.UseSeconday();
        punchHand.transform.position -= transform.forward * 0.75f;
    }


    [Client]
    public override void UseSeconday()
    {
        if (player.paused)
            return;

        if (timeSincePunch < punchCooldown)
            return;


        punchHand.transform.position += transform.forward * 0.75f;
        Invoke(nameof(Punch), punchDelay);
        timeSincePunch = 0;
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

        //player.playerCam.currentOffset -= summonRot * 1.3f;

        Vector3 spawnTrans = player.transform.position - Vector3.up * 3f;

        spawnTrans = Vector3Int.RoundToInt(spawnTrans);

        player.CmdSpawnObject(2, spawnTrans, Vector3.zero, false, false);
    }


    public override void Reload()
    {
        if (clipAmount < clipSize)
            reloading = true;
    }
}