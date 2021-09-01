using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCam : MonoBehaviour
{
    private Camera pCam;

    void Start()
    {

        Invoke(nameof(SetCam), 0.1f);
    }

    private void Update()
    {
        if(pCam == null)
            Invoke(nameof(SetCam), 0.1f);

        transform.localRotation = pCam.transform.rotation; 
    }

    void SetCam()
    {
        pCam = Camera.main;
    }
}
