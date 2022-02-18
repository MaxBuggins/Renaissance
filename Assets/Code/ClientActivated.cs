using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class ClientActivated : NetworkBehaviour
{
    public UnityEvent serverEvent;
    public UnityEvent clientEvent;


    public void TriggerEvent()
    {
        if (isServer)
        {
            RpcEvent();
            serverEvent.Invoke();
        }
        else
            CmdEvent();
    }

    public void CmdEvent()
    {
        serverEvent.Invoke();
        RpcEvent();
    }

    [ClientRpc]
    public void RpcEvent()
    {
        clientEvent.Invoke();
    }
}
