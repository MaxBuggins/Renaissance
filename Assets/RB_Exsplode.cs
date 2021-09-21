using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Exsplode : MonoBehaviour
{
    public float force = 1;
    public float radius = 1;

    public Rigidbody[] affected;
    
    void Start()
    {
        foreach(Rigidbody rb in affected)
        {
            rb.AddExplosionForce(force, transform.position, radius);
        }
    }
}
