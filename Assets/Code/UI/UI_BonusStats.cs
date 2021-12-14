using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_BonusStats : UI_Base
{
    public Image letterImage;
    public Image patternImage;

    public TextMeshProUGUI bonusStat1;
    public TextMeshProUGUI bonusStat2;
    public TextMeshProUGUI bonusStat3;

    public UI_Main uI_Main;


    public void Start()
    {
        InvokeRepeating("ShowInfo", 1.0f, 0.15f);
    }

    public override void UpdateInfo()
    {
        letterImage.sprite = uI_Main.player.playerClass.Leter;

        patternImage.sprite = uI_Main.player.playerClass.pattern;
        patternImage.color = uI_Main.player.playerStats.colour;
    }

    void ShowInfo()
    {
        if (ui_Main.player == null)
            return;

        bonusStat1.text = "v: (" + Mathf.Round(ui_Main.player.velocity.x * 10f) / 10f + ", " + Mathf.Round(ui_Main.player.velocity.y * 10f) / 10f + ", " + Mathf.Round(ui_Main.player.velocity.z * 10f) / 10f + ")";

        switch (uI_Main.player.playerClass.playerClass)
        {
            case (PlayerClass.Artist):
                {
                    break;
                }

            case (PlayerClass.Banker):
                {

                    bonusStat2.text = "";
                    bonusStat3.text = "";

                    break;
                }

            case (PlayerClass.Convict):
                {

                    if (uI_Main.player.fallTime < 0.3f)
                        bonusStat2.text = ("FallDmg: None");

                    else
                        bonusStat2.text = ("FallDmg: " + Mathf.Round(Mathf.Pow(uI_Main.player.fallTime, 1.6f) * 120));


                    bonusStat3.text = "";

                    break;
                }

            case (PlayerClass.Maid):
                {

                    bonusStat2.text = "";
                    bonusStat3.text = "";

                    break;
                }
        }
    }
}
