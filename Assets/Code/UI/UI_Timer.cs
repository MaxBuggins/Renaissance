using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class UI_Timer : UI_Base
{
    private TextMeshProUGUI timer;

    private void Start()
    {
        timer = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        TimeSpan time = TimeSpan.FromSeconds((float)NetworkTime.time);
        timer.text = time.ToString(@"mm\:ss");
    }
}
