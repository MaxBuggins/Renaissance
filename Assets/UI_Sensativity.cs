using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sensativity : UI_Base
{
    public Slider slider;

    private void Start()
    {
        slider.value = ui_Main.player.cameraObj.GetComponent<PlayerCamera>().mouseLookSensitivty;
    }

    public void UpdateValue(float value)
    {
        PlayerCamera pCam = ui_Main.player.GetComponentInChildren<PlayerCamera>();

        if (pCam != null)
           pCam.mouseLookSensitivty = value;
    }
}
