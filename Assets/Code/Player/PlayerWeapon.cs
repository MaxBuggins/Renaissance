using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//this script is for the local client to run only
public class PlayerWeapon : NetworkBehaviour
{

    private Controls controls;

    [Header("Unity Things")]
    private Player player;


    protected virtual void Start()
    {
        if (isServer)
            return;

        player = GetComponentInParent<Player>();

        controls = new Controls();

        controls.Game.Primary.performed += funny => UsePrimary();
        controls.Game.Seconday.performed += funnyer => UseSeconday();

        controls.Enable();
    }

    protected virtual void Update()
    {

    }

    public virtual void UsePrimary()
    {

    }

    public virtual void UseSeconday()
    {

    }
}
