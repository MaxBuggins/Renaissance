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
        if (ui_Main.players.Count <= scorePos - 1)
        {
            scoreText.text = ("No Player");
            scoreText.color = Color.clear;
            return;
        }

        Player topScorer = ui_Main.players[scorePos - 1];

        scoreText.text = (topScorer.playerName + ": "+ topScorer.score);
        scoreText.color = topScorer.playerColour;
    }
}
