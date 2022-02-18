using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using Mirror;

public class ArcSmash : MonoBehaviour
{
    public int swingDamage = 70;
    public float swingDistance;

    public float swingDuration;
    public AnimationCurve swingCurve;

    public Collider hurtCollider;
    public Hurtful hurtful;


    private GameObject centerAnchor; //rotate this
    public Transform centerObject; //follow this


    public void StartSwing()
    {
        centerObject = GetComponent<NetworkIdentity>().connectionToClient.identity.transform;

        centerAnchor = new GameObject("SwingCenter");
        centerAnchor.transform.parent = centerObject;
        centerAnchor.transform.localPosition = Vector3.zero;

        transform.parent = centerAnchor.transform;
        hurtful.damage = swingDamage;

        Tween.Rotate(centerAnchor.transform, transform.right * 180, Space.Self, swingDuration, 0, swingCurve, completeCallback: EndSwing);
        Tween.Rotate(transform, transform.right * 180, Space.World, swingDuration, 0, swingCurve);

        hurtCollider.isTrigger = true;
    }

    public void EndSwing()
    {
        hurtCollider.isTrigger = false;

        transform.parent = null;
        Destroy(centerAnchor);
    }
}
