using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class UI_Health : UI_Base
{
    public Image healthBar;
    public TextMeshProUGUI healthText;

    public override void UpdateInfo()
    {
        healthBar.fillAmount = (float)ui_Main.player.health / (float)ui_Main.player.maxHealth;
        healthText.text = ui_Main.player.health.ToString();
    }
}
