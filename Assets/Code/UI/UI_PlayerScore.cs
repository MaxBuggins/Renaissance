using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerScore : UI_Base
{
    public int scorePos = 1; //1st place score

    public TextMeshProUGUI scoreText;

    public override void UpdateInfo()
    {
        return;
        PlayerStats stats = ui_Main.players[scorePos - 1];

        if(stats == null)
        {
            scoreText.text = ("");
            return;
        }

        scoreText.text = (stats.name + ": "+ stats.GetScore());
        scoreText.color = stats.colour;
    }
}
