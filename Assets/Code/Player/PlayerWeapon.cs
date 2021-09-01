using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//this script is for the local client to run only
public class PlayerWeapon : NetworkBehaviour
{
    public bool canHoldPrimay = false;

    public int specialCost;

    [HideInInspector] public Controls controls;
    private bool primaryHeld = false;

    public bool specialIsActive = false;

    public float reloadPerstenage = 0; //for UI

    [Header("Unity Things")]
    public Player player;


    protected virtual void Start()
    {
        if (isServer || player.paused)
            return;

        player = GetComponentInParent<Player>();

        controls = new Controls();

        controls.Game.Primary.performed += funny => UsePrimary();
        controls.Game.Primary.canceled += funny => primaryHeld = false;

        controls.Game.Seconday.performed += funnyer => UseSeconday();

        controls.Game.Special.performed += funnyiest => UseSpecial();

        controls.Game.Reload.performed += kindaEpic => Reload();

        controls.Enable();
    }

    protected virtual void Update()
    {
        if (player.paused)
            return;

        if (canHoldPrimay == true && primaryHeld == true)
            UsePrimary();
    }

    public virtual void UsePrimary()
    {
        if (player.paused)
            return;

        primaryHeld = true;
    }

    public virtual void UseSeconday()
    {
        if (player.paused)
            return;
    }

    public virtual void UseSpecial()
    {
        if (player.paused)
            return;

        player.CmdAddSpecial(-specialCost);

        specialIsActive = true;
    }

    public virtual void EndSpecial()
    {
        if (player.paused)
            return;

        specialIsActive = false;
    }

    public virtual void Reload()
    {

    }
}
