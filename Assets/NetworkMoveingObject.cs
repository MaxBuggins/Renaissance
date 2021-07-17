using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class NetworkMoveingObject : NetworkBehaviour
{
    public enum MoveMode { constantDirection, backAndForth }
    public MoveMode moveMode = MoveMode.constantDirection;

    public float moveSpeed = 5;

    private int currentStop = 0;
    private int moveDirection = 1;

    private Vector3 orignalPos;
    public Transform[] path;
    private Vector3[] pathPos;

    private float TimeToReachDesternation;


    public override void OnStartServer()
    {
        orignalPos = transform.position;

        pathPos = new Vector3[path.Length];
        for(int i = 0; i < path.Length; i++)
        {
            pathPos[i] = path[i].localPosition;
        }


        if (moveMode == MoveMode.backAndForth)
        {
            ReachedDesternation();
        }
    }

    public override void OnStartClient()
    {
        orignalPos = transform.position;

        pathPos = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            pathPos[i] = path[i].localPosition;
        }

        getServerObject();
    }

    [Command]
    void getServerObject()
    {
        sendClientObject(currentStop, moveDirection, TimeToReachDesternation);
    }

    [ClientRpc]
    void sendClientObject(int _currentStop, int _moveDirection, float timeTillStart)
    {
        currentStop = _currentStop;
        moveDirection = _moveDirection;

        //makes sure it is started at same time as server
        Invoke(nameof(ReachedDesternation), timeTillStart - Time.time);
    }

    //for all clients and server
    void Update()
    {
        if (moveMode == MoveMode.constantDirection)
            transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

    void ReachedDesternation()
    {
        if (moveMode == MoveMode.backAndForth)
        {
            //checks if end of path then reverses it
            if (currentStop + moveDirection >= path.Length || currentStop + moveDirection < 0)
            {
                moveDirection = -moveDirection;
            }
        }
        currentStop += moveDirection;

        Tween.Position(transform, orignalPos + pathPos[currentStop], moveSpeed, 0, completeCallback: ReachedDesternation);
        TimeToReachDesternation = Time.time + moveSpeed;
    }
}
