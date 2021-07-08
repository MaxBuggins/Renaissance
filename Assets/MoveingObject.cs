using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingObject : MonoBehaviour
{
    public float moveSpeed = 5;


    void Start()
    {
    }

    //for all clients and server
    void Update()
    {
        transform.position += transform.right * moveSpeed * Time.deltaTime; 
    }
}
