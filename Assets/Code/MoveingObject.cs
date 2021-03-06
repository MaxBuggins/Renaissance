using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MoveingObject : MonoBehaviour
{
    public enum MoveMode { constantDirectionRight, constantDirectionForward, constantDirectionUp }
    public MoveMode moveMode = MoveMode.constantDirectionRight;
    
    public float moveSpeed = 5;
    public float startDelay = 0;

    public bool localDistance = false;
    public float stopDistance = 0;

    private Vector3 orginPos;
    public Transform[] path;

    void Start()
    {
        orginPos = transform.position;

        if (stopDistance <= 0)
            stopDistance = Mathf.Infinity;

        if (startDelay > 0)
        {
            enabled = false;
            Invoke(nameof(Enable), startDelay);
        }
    }

    public void Enable()
    {
        enabled = true;
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
            case (MoveMode.constantDirectionUp):
                {
                    transform.position += transform.up * moveSpeed * Time.deltaTime;
                    break;
                }
        }


        if (localDistance)
        {
            if (Vector3.Distance(Vector3.zero, transform.localPosition) > stopDistance)
            {
                enabled = false;
            }
        }

        else
            if (Vector3.Distance(orginPos, transform.position) > stopDistance)
                enabled = false;
    }
}
