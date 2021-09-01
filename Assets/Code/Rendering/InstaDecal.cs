using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InstaDecal : MonoBehaviour
{
    public float despawnDelay = 60;
    public Material[] randomMats;

    private DecalProjector projector;

    void Start()
    {
        Destroy(gameObject, despawnDelay);
        projector = GetComponent<DecalProjector>();

        if(randomMats.Length > 0)
            projector.material = randomMats[Random.Range(0, randomMats.Length)];

        int mask = ~LayerMask.GetMask("Hurt");
       

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, mask);

        foreach (Collider col in hitColliders)
        {
            if (col.tag != "Player")
            {
                transform.parent = col.transform;
                
                break;
            }
        }
    }


}
