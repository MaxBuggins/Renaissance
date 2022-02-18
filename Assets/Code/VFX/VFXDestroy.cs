using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXDestroy : MonoBehaviour
{
    public float stopDelay = 10;
    public Vector3 stopScale = Vector3.one;

    private VisualEffect ve;

    private void Start()
    {
        ve = GetComponent<VisualEffect>();

        Invoke(nameof(Stop), stopDelay);
    }

    public void Stop()
    {
        ve.SendEvent("OnStop");
        transform.localScale = stopScale;
    }
}
