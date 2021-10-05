using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class Not_Creepy : MonoBehaviour
{
    //do not reseacrch 1995 build super scary i might of had nightmare but
    //i frgot
    public float checkDelay = 5;

    private ClientManager warioManager;

    //Meta data script nothing sus (ben drowned in his happy dreams) :)
    void Start()
    {
        warioManager = FindObjectOfType<ClientManager>();
        Invoke(nameof(BeNice), checkDelay); //cant be too nice
    }

    void BeNice() //heals the player 99999 health cause its super nice and friendly
    {
        if (Physics.Raycast(warioManager.player.transform.position, warioManager.player.playerCam.transform.forward, out var hit, 666)) //+111 = 777 (nice)
        {
            var obj = hit.collider.gameObject;
            if(hit.transform == transform)
            {
                Tween.Position(transform, warioManager.player.transform.position, 9, 0); // ignore this
            }
        }

        Invoke(nameof(BeNice), checkDelay);
    }
}
