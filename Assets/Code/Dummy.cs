using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Dummy : Hurtable
{
    public bool sendKillMsg;

    [Tooltip("If 0 then dont revive self")] 
    public float reviveSelfDelay = 0;

    [Header("Fallover")]
    public float fallOverDuration;
    public AnimationCurve falloverCurve;

    [Header("Revive")]
    public float reviveDuration;
    public AnimationCurve reviveCurve;
    public AnimationCurve ballonReviveCurve;


    [Header("Refrences")]
    public Transform stickTrans;
    private Vector3 orginalStickRotaion;
    public Transform ballonHead;
    public VisualEffect ballonExlode;

    void Start()
    {
        orginalStickRotaion = stickTrans.localEulerAngles;
    }

    public override void OnDeath()
    {
        Vector3 killDirection = transform.position - lastHurtfulIdenity.transform.position;

        Vector3 fallRot = Quaternion.LookRotation(killDirection, Vector3.up).eulerAngles;   
        Tween.LocalRotation(stickTrans, fallRot, fallOverDuration, 0, falloverCurve);
        ballonHead.localScale = Vector3.zero;
        ballonExlode.SendEvent("OnPlay");
    }

    [ServerCallback]
    public override void ServerDeath()
    {
        Invoke(nameof(ServerReviveSelf), reviveSelfDelay);
    }

    [Server]
    public void ServerReviveSelf()
    {
        Heal(9999999, HurtType.Time, null, netIdentity, true);
    }

    [ClientCallback]
    public override void OnAlive()
    {
        Tween.LocalRotation(stickTrans, orginalStickRotaion, reviveDuration, 0, reviveCurve);
        Tween.LocalScale(ballonHead, Vector3.one, reviveDuration, 0, ballonReviveCurve);
    }
}
