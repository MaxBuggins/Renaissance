using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Special : UI_Base
{
    public Image specialBar;
    public TextMeshProUGUI specialText;

    public override void UpdateInfo()
    {
        specialBar.fillAmount = (float)ui_Main.player.special / (float)ui_Main.player.maxSpecial;
        specialText.text = ui_Main.player.special.ToString();
    }
}
