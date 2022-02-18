using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class CollisionrEvent : MonoBehaviour
{
    public UnityEvent serverEvent;
    public UnityEvent clientEvent;


    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Untagged")
        {
            serverEvent.Invoke();
        }
    }
}
