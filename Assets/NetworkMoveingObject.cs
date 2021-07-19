using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;
using Pixelplacement.TweenSystem;

public class NetworkMoveingObject : NetworkBehaviour
{
    public enum MoveMode { constantDirection, backAndForth }
    public MoveMode moveMode = MoveMode.constantDirection;

    public float moveSpeed = 5;

    private int currentStop = 0;
    private int moveDirection = 1;

    public Vector3 orignalPos;
    public Transform[] path;
    private Vector3[] pathPos;

    private float TimeToReachDesternation;
    private TweenBase moveTween;

    private NetworkIdentity identity;


    private void OnDrawGizmos()
    {
        orignalPos = transform.position;
    } //temp fix proberly

    public override void OnStartServer()
    {
        identity = GetComponent<NetworkIdentity>();

        pathPos = new Vector3[path.Length];
        for(int i = 0; i < path.Length; i++)
        {
            pathPos[i] = path[i].localPosition;
        }


        if (moveMode == MoveMode.backAndForth)
        {
            ReachedDesternation();
        }

       // identity.AssignClientAuthority();
    }

    public override void OnStartClient()
    {
        identity = GetComponent<NetworkIdentity>();

        pathPos = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            pathPos[i] = path[i].localPosition;
        }

        CmdgetServerObject();
    }



    [Command] //doesnt work as client doesnt have authority HELP
    void CmdgetServerObject()
    {
        RpcsendClientObject(currentStop, moveDirection, TimeToReachDesternation);
    }

    [ClientRpc]
    void RpcsendClientObject(int _currentStop, int _moveDirection, double timeTillStart)
    {
        if (moveTween != null) //TEMP FIX
            return;

        currentStop = _currentStop;
        moveDirection = _moveDirection;

        //makes sure it is started at same time as server
        Invoke(nameof(ReachedDesternation), (float)(timeTillStart - NetworkTime.time));
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

        moveTween = Tween.Position(transform, orignalPos + pathPos[currentStop], moveSpeed, 0, completeCallback: ReachedDesternation);
        TimeToReachDesternation = (float)NetworkTime.time + moveSpeed;

        if(identity.isServer) //Fix to CALL WHEN A CLIENT JOINS
            RpcsendClientObject(currentStop, moveDirection, TimeToReachDesternation);
    }
}
