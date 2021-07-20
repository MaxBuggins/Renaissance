using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaDecal : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode

        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(transform.position, transform.localScale * 2);
    }

    void Start()
    {
        int mask = ~LayerMask.GetMask("Hurt");

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, mask);

        if(hitColliders.Length > 0)
            transform.parent = hitColliders[0].transform;

    }
}
