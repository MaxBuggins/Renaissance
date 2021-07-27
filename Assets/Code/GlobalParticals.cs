using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalParticals : MonoBehaviour
{
    private Transform follow;

    private ParticleSystem particles;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (follow == null)
        {
            if(Camera.main != null)
                follow = Camera.main.transform;
        }
        else
            transform.position = follow.position;
    }
}
