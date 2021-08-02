using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BankerWeapon : PlayerWeapon
{
    [Header("Punch Propertys")]
    public float punchDelay;
    public float punchCooldown;
    private float timeSincePunch;

    public float punchVelocity = 6;

    [Header("Shoot Propertys")]
    public float fireRate = 0.4f;
    private float fireTime = 0;

    public int clipSize = 6;
    private int clipAmount;

    private bool reloading = false;
    public float reloadRate = 2;
    private float reloadTime = 0;

    [Header("BriefCaseSmash Propertys")]
    public float returnDelay = 1;
    private float timeSinceThrow;


    [Header("Weapon Refrences")]
    public GameObject punchHand;
    public GameObject briefCase;

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

        timeSinceThrow += Time.deltaTime;

        reloadPerstenage = reloadTime / reloadRate;

        if(reloading)
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
        if (player.paused)
            return;

        if (timeSincePunch < punchCooldown || timeSinceThrow < returnDelay)
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

        if (fireTime < fireRate)
            return;

        if (clipAmount <= 0)
        {
            reloading = true;
            return;
        }

        clipAmount -= 1;

        player.CmdSpawnObject(1, shootPos.position, shootPos.eulerAngles, false, false);
        base.UseSeconday();
        fireRate = 0;
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.paused)
            return;

        //whould like base.useSpecial to run this but havent figured that out yet give me 7 years
        if (player.special - specialCost < 0 || timeSinceThrow < returnDelay) //not special enough falount 7 refrence (ADIAN HOLDSWORTH)
            return;

        base.UseSpecial();

        timeSinceThrow = 0;
        briefCase.SetActive(false);
        punchHand.transform.position += transform.forward * 0.75f;

        Invoke(nameof(ActivateBriefCase), returnDelay);

        player.CmdSpawnObject(2, transform.position + transform.forward, shootPos.eulerAngles, false, false);
    }

    void ActivateBriefCase()
    {
        punchHand.transform.position -= transform.forward * 0.75f;
        briefCase.SetActive(true);
    }

}