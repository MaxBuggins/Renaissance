using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MaidWeapon : PlayerWeapon
{
    [Header("Ice Breath")]
    public float punchDelay;
    public float punchCooldown;
    private float timeSincePunch;

    public float punchVelocity = 6;

    [Header("PlateThrow Propertys")]
    public float fireRate = 0.25f;
    private float fireTime = 0;

    public Vector3 throwRot;

    [Header("IceSummon Propertys")]
    public LayerMask summonMask = -1;
    public Vector3 summonRot;
    public enum IceShape {cube, sphere, pyrsim}
    public IceShape currentShape = IceShape.cube;


    [Header("Weapon Refrences")]
    public GameObject punchHand;

    public Transform shootPosPlate;
    public Transform shootPosColdDust;

    protected override void Start()
    {
        player = GetComponentInParent<Player>();

        base.Start();
    }

    protected override void Update()
    {
        if (player.paused)
            return;

        fireTime += Time.deltaTime;

        timeSincePunch += Time.deltaTime;

        base.Update();
    }

    [Client]
    public override void UsePrimary()
    {
        if (player.paused)
            return;

        if (fireTime < fireRate || timeSincePunch < punchCooldown)
            return;



        player.CmdSpawnObject(1, shootPosPlate.position, shootPosPlate.eulerAngles, false, false);


        base.UsePrimary();
        fireTime = 0;
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

    void Punch()
    {
        player.playerCam.currentOffset -= throwRot;

        player.CmdSpawnObject(0, shootPosColdDust.position, shootPosColdDust.eulerAngles, false, false);
        player.velocity += transform.forward * punchVelocity;
        base.UseSeconday();
        punchHand.transform.position -= transform.forward * 0.75f;
    }

    [Client]
    public override void UseSpecial()
    {
        if (player.paused)
            return;

        int cost = specialCost;

        if (currentShape == IceShape.cube)
            cost *= 3;

        //whould like base.useSpecial to run this but havent figured that out yet give me 7 years
        if (player.special - cost < 0) //not special enough falount 7 refrence (ADIAN HOLDSWORTH)
            return;

        player.CmdAddSpecial(-cost);
        specialIsActive = true;

        //player.playerCam.currentOffset -= summonRot * 1.3f;

        Vector3 spawnTrans = player.transform.position;
        Vector3 spawnRot = Vector3.zero;

        switch (currentShape)
        {
            case (IceShape.cube):
                {
                    spawnTrans = player.transform.position - Vector3.up * 3.25f;
                    break;
                }
            case (IceShape.sphere):
                {
                    spawnTrans = player.transform.position + transform.forward * 7f;
                    break;
                }
            case (IceShape.pyrsim):
                {
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position, transform.forward);

                    if (Physics.Raycast(ray: ray, maxDistance: 50, hitInfo: out hit, layerMask: summonMask))
                    {
                        spawnTrans = hit.point;

                        spawnRot = Quaternion.FromToRotation(Vector3.up, hit.normal).eulerAngles;
                    }
                    else
                    {
                        spawnTrans = player.transform.position + transform.forward * 50f - Vector3.up;
                    }
                    break;
                }
        }


        spawnTrans = Vector3Int.RoundToInt(spawnTrans);

        player.CmdSpawnObject(2 + (int)currentShape, spawnTrans, spawnRot, false, false);
    }


    public override void Reload()
    {
        int length = Enum.GetNames(typeof(IceShape)).Length;
        if ((int)currentShape >= length - 1)
            currentShape = 0;
        else
            currentShape = currentShape + 1;

        player.uIMain.UIUpdate();
    }
}