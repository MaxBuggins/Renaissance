using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateingObject : MonoBehaviour
{
    public Vector3 constantRot;


    void Start()
    {
        
    }

    void Update()
    {
        transform.eulerAngles += constantRot * Time.deltaTime;
    }
}
