using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMoveingObject : NetworkBehaviour
{
    public enum MoveMode { constantDirection, backAndForth }
    public MoveMode moveMode = MoveMode.constantDirection;

    public float moveSpeed = 5;

    public Vector3 originPos;
    public Transform[] path;
    private Vector3[] pathPos;

    private NetworkIdentity identity;


    private void OnDrawGizmos() //temp fix proberly
    {
        originPos = transform.position;
    }

    //for all clients and server
    void Update()
    {
        switch (moveMode)
        {
            case (MoveMode.constantDirection):
                {
                    transform.position += transform.right * moveSpeed * (float)NetworkTime.time;
                    break;
                }

            case (MoveMode.backAndForth):
                {
                    //float yPos = originPos.y + Mathf.Sin((float)NetworkTime.time * moveSpeed) * sinHeight;

                    //transform.position = new Vector3(originPos.x, yPos, originPos.z);
                    break;
                }
        }
    }
}
