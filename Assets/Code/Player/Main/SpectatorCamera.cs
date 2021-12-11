using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpectatorCamera : PlayerBase
{
    private Controls controls;

    private NetworkIdentity identity;

    void Start() //ignore the green
    {
        base.Start(); //fixes the green
        identity = GetComponent<NetworkIdentity>();
        if (!identity.isLocalPlayer) //only the spectator uses this
            gameObject.SetActive(false);

        controls = new Controls();

        controls.Game.ChangeClass.performed += funnyiest => uIMain.DisplayClassDetails(0);

        controls.Enable();
    }
}
