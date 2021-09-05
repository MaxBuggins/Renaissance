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
        switch (ui_Main.player.playerClass.playerClass)
        {
            case (PlayerClass.Maid):
                {
                    int index = (int)ui_Main.player.playerWeapon.GetComponent<MaidWeapon>().currentShape;
                    specialIcon.sprite = specialSprites[index];
                    break;
                }
            case (PlayerClass.Banker):
                {
                    specialIcon.sprite = specialSprites[3];
                    break;
                }

            case (PlayerClass.Convict):
                {
                    specialIcon.sprite = specialSprites[3];
                    break;
                }
        }

        //ui_Main.player.playerWeapon
        specialBar.fillAmount = (float)ui_Main.player.special / (float)ui_Main.player.maxSpecial;
        specialText.text = ui_Main.player.special.ToString();
    }
}
