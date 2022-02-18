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

    //public float punchVelocity = 6;

    public Vector3 swingRot;

    [Header("Shoot Propertys")]
    public int bulletCost = 0;
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

    [Header("Lotto Options")]
    public bool lottoSpecial = false;
    private PlayerLotto lotto;


    [Header("Weapon Refrences")]
    public GameObject briefCase;

    public Transform shootPos;
    public LayerMask shootHitLayer;

    private Animator animator;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
        lotto = player.GetComponent<PlayerLotto>();

        timeSinceThrow = returnDelay;
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

        if (reloading)
        {
            reloadTime += Time.deltaTime;

            if (reloadTime > reloadRate)
            {
                clipAmount = clipSize;
                reloading = false;
                reloadTime = 0;

                animator.ResetTrigger("Primary");
            }
        }

        timeSincePunch += Time.deltaTime;

        base.Update();
    }

    [Client]
    public override void UseSeconday()
    {
        if (player.paused)
            return;

        if (timeSincePunch < punchCooldown || timeSinceThrow < returnDelay)
            return;


        Invoke(nameof(Punch), punchDelay);
        base.UseSeconday();
        timeSincePunch = 0;
    }

    void Punch()
    {
        player.playerCam.currentOffset -= swingRot;

        player.CmdSpawnObject(0, Vector3.zero, transform.eulerAngles, false, true);
        //player.velocity += transform.forward * punchVelocity;
    }


    [Client]
    public override void UsePrimary()
    {
        if (player.paused || reloading)
            return;

        if (fireTime < fireRate)
            return;

        if (clipAmount <= 0 && clipSize > 0)
        {
            animator.SetTrigger("Reload");
            reloading = true;
            return;
        }

        if (bulletCost > 0)
        {
            if (player.special - bulletCost < 0)
                return;

            player.CmdAddSpecial(-bulletCost);

        }
        clipAmount -= 1;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //Debug.DrawRay(transform.position, transform.forward * 1000, Color.red, 10);

        if (Physics.Raycast(ray, out hit, 7000, shootHitLayer, QueryTriggerInteraction.Ignore))
        {
            Vector3 _direction = (hit.point - shootPos.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);

            player.CmdSpawnObject(1, shootPos.position, _lookRotation.eulerAngles, false, false);

        }

        else
            player.CmdSpawnObject(1, shootPos.position, shootPos.eulerAngles, false, false);

        base.UsePrimary();
        fireTime = 0;

        CheckIfReload();
    }

    public void CheckIfReload()
    {
        if (clipAmount <= 0 && clipSize > 0)
        {
            animator.SetTrigger("Reload");
            reloading = true;
        }
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

        if (lottoSpecial == true)
        {
            lotto.CmdEnterLotto();
            return;
        }

        player.playerCam.currentOffset -= swingRot * 1.3f;

        timeSinceThrow = 0;
        //briefCase.SetActive(false);

        Invoke(nameof(ActivateBriefCase), returnDelay);

        player.CmdSpawnObject(2, transform.position + transform.forward, shootPos.eulerAngles, false, false);
    }

    void ActivateBriefCase()
    {
        briefCase.SetActive(true);
    }

    public override void Reload()
    {
        if (clipAmount < clipSize)
        {
            reloading = true;
            animator.SetTrigger("Reload");
        }
    }
}