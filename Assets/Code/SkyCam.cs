using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCam : MonoBehaviour
{
    public float movementScale = 1;

    private Vector3 offset;
    private Camera pCam;

    void Start()
    {
        Invoke(nameof(SetCam), 0.2f);
        enabled = false;
    }

    private void Update()
    {
        if (pCam == null)
        {
            pCam = Camera.main;
            return;
        }

        transform.position = (pCam.transform.position * movementScale) - offset;
        transform.localRotation = pCam.transform.rotation; 
    }

    void SetCam()
    {
        pCam = Camera.main;
        offset = Vector3.zero - transform.position;

        enabled = true;
    }
}
