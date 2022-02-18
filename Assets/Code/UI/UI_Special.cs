using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Special : UI_Base
{
    public Image specialBar;
    public TextMeshProUGUI specialText;

    public Image specialIcon;
    public Sprite[] specialSprites;

    public void start()
    {

    }

     public override void UpdateInfo()
    {
        string displayString = ui_Main.player.special.ToString();
        float fillAmount = (float)ui_Main.player.special / (float)ui_Main.player.maxSpecial;

        switch (ui_Main.player.playerClass.playerClass)
        {
            case (PlayerClass.Maid):
                {
                    int index = (int)ui_Main.player.playerWeapon.GetComponent<MaidWeapon>().currentShape;
                    specialIcon.sprite = specialSprites[3 + index];
                    break;
                }
            case (PlayerClass.Banker):
                {
                    specialIcon.sprite = specialSprites[1];
                    fillAmount = 1;

                    if(ui_Main.player.playerClass.upperCase)
                        displayString = "$" + ((float)ui_Main.player.special / 100).ToString();

                    else
                        displayString = "$" + (ui_Main.player.special * 100).ToString();

                    break;
                }

            case (PlayerClass.Convict):
                {
                    specialIcon.sprite = specialSprites[2];
                    break;
                }
        }

        specialBar.fillAmount = fillAmount;
        specialText.text = displayString;
    }
}
