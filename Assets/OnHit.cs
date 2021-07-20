using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHit : MonoBehaviour
{
    public int destoryOnHits = 2;

    public GameObject hitObject;

    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        destoryOnHits -= 1;

        GameObject obj = Instantiate(hitObject, collision.contacts[0].point, transform.rotation, null);

        var objMat = obj.GetComponent<Material>();

        if (objMat != null)
            objMat.color = GetComponent<Renderer>().material.color;

        if (destoryOnHits <= 0)
            Destroy(gameObject);
    }
}
