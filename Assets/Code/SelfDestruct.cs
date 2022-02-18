using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.VFX;

public class SelfDestruct : MonoBehaviour
{
    public bool unParentOnStart = false;

    public float destroyDelay = 1f;
    public bool destoryOnServer = false;

    public Transform[] unParentTransOnDestory;

    void Start()
    {
        if (unParentOnStart == true)
            transform.parent = null;

        Invoke(nameof(DestroySelf), destroyDelay);
    }

    void DestroySelf()
    {
        if (destoryOnServer == false)
        {
            //because unity disables all components even in children on destory so we have to enable to ones we want (all)
            foreach (Transform trans in unParentTransOnDestory)
            {
                trans.parent = null;

            }

            Destroy(gameObject);
        }
        else
            NetworkServer.Destroy(gameObject);
    }

}
