using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    public Player player;

    public UI_Base[] uiBases;

    public void UIUpdate()
    {
        foreach (UI_Base ui in uiBases)
            ui.UpdateInfo();
    }
}
