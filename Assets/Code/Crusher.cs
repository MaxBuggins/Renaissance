using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class Crusher : NetworkBehaviour
{

    [Header("Crush")]
    private Vector3 startPos;
    public Transform endTrans;

    public float crushDelay;
    public float crushDuration;

    public AnimationCurve crushCurve;

    private Animator animator;


    [Header("Server Control")]

    public Vector2 randomDelay; //i forgot the better word than delay for this context HELP

    private void Start()
    {
        animator = GetComponent<Animator>();

        startPos = transform.position;

        Ready();
    }

    [Server]
    public void StartCrush()
    {
        RpcStartCrush();

        Invoke(nameof(MoveCrush), crushDelay);
    }

    [ClientRpc]
    public void RpcStartCrush()
    {
        animator.SetTrigger("Step");
        Invoke(nameof(MoveCrush), crushDelay);
    }

    void MoveCrush()
    {
        animator.SetBool("Move", true);
        Tween.Position(transform, endTrans.position, crushDuration, 0, crushCurve, completeCallback: Crush);
    }

    void Crush()
    {
        animator.SetTrigger("Step");
        Tween.Position(transform, startPos, crushDuration, 1, crushCurve, completeCallback: Ready);
    }

    void Ready()
    {
        animator.SetBool("Move", false);

        if(isServer)
            Invoke(nameof(StartCrush), Random.Range(randomDelay.x, randomDelay.y));
    }

    private void OnDrawGizmos() //Looks cool in scene view
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, endTrans.position);

    }
}
