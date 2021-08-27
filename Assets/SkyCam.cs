using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCam : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SetParent), 0.1f);
    }

    void SetParent()
    {
        transform.parent = Camera.main.transform;
        transform.localEulerAngles = Vector3.zero;
    }
}
