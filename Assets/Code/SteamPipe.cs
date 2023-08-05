using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamPipe : Hurtable
{
    public GameObject smokeLeakObject;
    public GameObject smokeAreaObject;
    public override void OnHurt(int damage, Vector3 hitPos = default(Vector3), Vector3 hitRot = default(Vector3)) //do somethin or other to a toaster
    {
        Instantiate(smokeLeakObject, hitPos, Quaternion.Euler(hitRot));

        return;
        if (isServer)
        {
            GameObject spawned = Instantiate(smokeAreaObject, hitPos, Quaternion.Euler(hitRot));
            NetworkServer.Spawn(spawned);
        }
    }
}
