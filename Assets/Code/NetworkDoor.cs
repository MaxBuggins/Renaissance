using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;
using UnityEngine.Events;

public class NetworkDoor : NetworkBehaviour
{
    [Header("Stats")]
    [SyncVar(hook = nameof(OnDoorChange))] public bool isOpen = false;
    private int playerCount;


    [Header("Movement")]
    public float moveDelay = 0;
    public float moveDuration = 0.5f;
    public AnimationCurve moveCurve;


    [Header("Door")]
    public Transform door;

    public Vector3 doorOpenPos;
    public Vector3 doorClosedPos;

    public UnityEvent openEvent;
    public UnityEvent closeEvent;

    void Start()
    {
        if (isOpen)
        {
            door.localPosition = doorOpenPos;
            FinishOpen();
        }

        else
        {
            door.localPosition = doorClosedPos;
            FinishClose();
        }
    }

    public void OnDoorChange(bool _Old, bool _New)
    {
        if (_New == true)
            Open();
        else
            Close();
    }


    public void Open()
    {
        Tween.LocalPosition(door, doorOpenPos, moveDuration, 0, moveCurve, completeCallback: FinishOpen);
    }

    public void Close()
    {
        Tween.LocalPosition(door, doorClosedPos, moveDuration, 0, moveCurve, completeCallback: FinishClose);
    }

    void FinishOpen()
    {
        openEvent.Invoke();
    }

    void FinishClose()
    {
        closeEvent.Invoke();
    }


    [Server]
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        
        if(player != null)
        {
            isOpen = true;
            Open();
            playerCount++;
        }
    }

    [Server]
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            playerCount--;

            if (playerCount <= 0)
            {
                isOpen = false;
                Close();
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0);
        Gizmos.DrawLine(transform.position + doorOpenPos, transform.position + doorClosedPos);
    }
}
