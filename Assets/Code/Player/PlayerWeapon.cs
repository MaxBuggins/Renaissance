using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//this script is for the local client to run only
public class PlayerWeapon : NetworkBehaviour
{
    public bool canHoldPrimay;

    public int specialCost;

    private Controls controls;
    private bool primaryHeld = false;

    [Header("Unity Things")]
    public Player player;


    protected virtual void Start()
    {
        if (isServer)
            return;

        player = GetComponentInParent<Player>();

        controls = new Controls();

        controls.Game.Primary.performed += funny => UsePrimary();
        controls.Game.Primary.canceled += funny => primaryHeld = false;

        controls.Game.Seconday.performed += funnyer => UseSeconday();

        controls.Game.Special.performed += funnyiest => UseSpecial();

        controls.Enable();
    }

    protected virtual void Update()
    {
        if (canHoldPrimay == true && primaryHeld == true)
            UsePrimary();
    }

    public virtual void UsePrimary()
    {
        primaryHeld = true;
    }

    public virtual void UseSeconday()
    {

    }

    public virtual void UseSpecial()
    {

    }

    public virtual void EndSpecial()
    {

    }
}
