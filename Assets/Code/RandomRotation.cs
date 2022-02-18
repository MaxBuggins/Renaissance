using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public Vector3 minRandomRot;
    public Vector3 maxRandomRot;

    void Start()
    {
        transform.eulerAngles = new Vector3(Random.Range(minRandomRot.x, maxRandomRot.x),
            Random.Range(minRandomRot.y, maxRandomRot.y),
            Random.Range(minRandomRot.z, maxRandomRot.z));
    }
}
