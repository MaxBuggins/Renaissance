using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MoveingObject : MonoBehaviour
{
    public enum MoveMode { constantDirection, backAndForth }
    public MoveMode moveMode = MoveMode.constantDirection;
    
    public float moveSpeed = 5;

    private Vector3 startPos;
    public Transform[] path;

    void Start()
    {
        startPos = transform.position;
    }

    //for all clients and server
    void Update()
    {
        if(moveMode == MoveMode.constantDirection)
            transform.position += transform.right * moveSpeed * Time.deltaTime; 
    }
}
