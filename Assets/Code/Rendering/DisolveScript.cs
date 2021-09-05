using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisolveScript : MonoBehaviour
{
    public float startValue;
    public float speed = 1;
    private float time;

    private Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        render.material.SetFloat("_DisAmount", startValue + (time * speed));
    }
}
