using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDelay : MonoBehaviour
{
    public float delay;
    [SerializeField] private UnityEvent delayedEvent;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Event), delay);
    }

    void Event()
    {
        delayedEvent.Invoke();
    }
}
