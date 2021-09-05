using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour
{
    public UI_Main ui_Main;

    void Awake()
    {
        ui_Main = GetComponentInParent<UI_Main>(); 
    }

    public virtual void UpdateInfo()
    {

    }
}
