using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisableInPlay : MonoBehaviour
{
    void Start()
    {
        if (Application.isPlaying)
        {
            GetComponent<Renderer>().material.color = Color.clear;
        }
    }
}
