using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;

public class UI_Health : UI_Base
{
    public Image healthBar;
    public TextMeshProUGUI healthText;

    public float hurtScale = 1.25f;
    public float hurtDuration;
    public AnimationCurve hurtCurve;

    public override void UpdateInfo()
    {
        float fill = (float)ui_Main.player.health / (float)ui_Main.player.maxHealth;

        if (fill != healthBar.fillAmount)
        {
            healthBar.transform.localScale = Vector3.one; //reset the scale incase called too quickly
            healthText.transform.localScale = Vector3.one;
            Tween.LocalScale(healthBar.transform, Vector3.one * hurtScale / 2, hurtDuration, 0, hurtCurve);
            Tween.LocalScale(healthText.transform, Vector3.one * hurtScale, hurtDuration, 0, hurtCurve);

            healthBar.fillAmount = (float)ui_Main.player.health / (float)ui_Main.player.maxHealth;
            healthText.text = ui_Main.player.health.ToString();
        }
    }
}
