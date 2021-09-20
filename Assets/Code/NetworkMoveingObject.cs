using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMoveingObject : MonoBehaviour
{
    public enum MoveMode { constantDirectionRight, constantDirectionForward, loop, resetLoop}
    public MoveMode moveMode = MoveMode.constantDirectionRight;

    public float startTime = 0;
    public float moveSpeed = 5;
    public float moveDelay = 0;

    private Vector3 orginPos; //I trust mirror is efficent
    public Transform[] path;

    private NetworkIdentity identity;


    private void OnDrawGizmos() //Looks cool in scene view
    {
        Gizmos.color = Color.green;

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
        startTime -= (float)NetworkTime.time;
        orginPos = transform.position;
    }

    //for all clients and server
    void FixedUpdate()
    {
        float time = (float)NetworkTime.time + startTime;

        float relativePos = (time * moveSpeed) / path.Length;

        switch (moveMode)
        {
            case (MoveMode.constantDirectionRight):
                {
                    transform.position = orginPos + transform.right * moveSpeed * time;
                    break;
                }

            case (MoveMode.constantDirectionForward):
                {
                    transform.position = orginPos + transform.forward * moveSpeed * time;
                    break;
                }

            case (MoveMode.loop):
                {
                    int posL = (int)Mathf.Repeat(Mathf.Floor(relativePos), path.Length); //from this pos
                    int posH = (int)Mathf.Repeat(Mathf.Ceil(relativePos), path.Length); //to this pos

                    transform.position = orginPos + LerpByDistance(path[posL].localPosition, path[posH].localPosition,
                        relativePos - Mathf.Floor(relativePos));

                    break;
                }

            case (MoveMode.resetLoop):
                {
                    relativePos = Mathf.Repeat(relativePos, path.Length - 1);

                    int posL = (int)Mathf.Floor(relativePos); //from this pos
                    int posH = (int)Mathf.Ceil(relativePos); //to this pos

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
