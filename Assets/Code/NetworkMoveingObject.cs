using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMoveingObject : MonoBehaviour
{
    public enum MoveMode { constantDirectionRight, constantDirectionForward, loop}
    public MoveMode moveMode = MoveMode.constantDirectionRight;

    

    public float moveSpeed = 5;

    private Vector3 orginPos; //I trust mirror is efficent
    public Transform[] path;

    private NetworkIdentity identity;


    private void OnDrawGizmos() //Looks cool in scene view
    {
        Gizmos.color = Color.green;

        //Gizmos.DrawLine(transform.position, endPosTransform[0].position);

        if (path.Length > 1)
        {
            int n = 0;
            foreach (Transform trans in path)
            {
                n++;
                if (n < path.Length)
                {
                    Gizmos.DrawLine(path[n - 1].position, path[n].position);
                }
            }
        }
    }

    void Start()
    {
        orginPos = transform.position;
    }

    //for all clients and server
    void FixedUpdate()
    {
        switch (moveMode)
        {
            case (MoveMode.constantDirectionRight):
                {
                    transform.position = transform.right * moveSpeed * (float)NetworkTime.time;
                    break;
                }

            case (MoveMode.constantDirectionForward):
                {
                    transform.position = transform.forward * moveSpeed * (float)NetworkTime.time;
                    break;
                }

            case (MoveMode.loop):
                {
                    float relativePos = ((float)NetworkTime.time * moveSpeed) / path.Length;


                    int posL = (int)Mathf.Repeat(Mathf.Floor(relativePos), path.Length);
                    int posH = (int)Mathf.Repeat(Mathf.Ceil(relativePos), path.Length);

                    transform.position = orginPos + LerpByDistance(path[posL].localPosition, path[posH].localPosition,
                        relativePos - Mathf.Floor(relativePos));

                    break;
                }
        }
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 pos = x * (B - A) + A;
        return pos;
    }
}
