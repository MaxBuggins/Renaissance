using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MoveingObject : MonoBehaviour
{
    public enum MoveMode { constantDirectionRight, constantDirectionForward }
    public MoveMode moveMode = MoveMode.constantDirectionRight;
    
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

        switch (moveMode)
        {
            case (MoveMode.constantDirectionRight):
                {
                    transform.position += transform.right * moveSpeed * Time.deltaTime;
                    break;
                }
            case (MoveMode.constantDirectionForward):
                {
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                }
        }
        }
}
