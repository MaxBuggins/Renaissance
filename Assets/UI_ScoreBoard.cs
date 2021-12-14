using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ScoreBoard : UI_Base
{
    public GameObject scoreRowPrefab;
    public Sprite square;

    public List<UI_scoreRow> scoreRows = new List<UI_scoreRow>();

    private void OnEnable()
    {
        UpdateInfo();
    }

    public override void UpdateInfo()
    {
        //Have the right number of player rows
        while (scoreRows.Count != ui_Main.players.Count)
        {
            if (scoreRows.Count > ui_Main.players.Count)
            {
                UI_scoreRow row = scoreRows[scoreRows.Count - 1];
                scoreRows.Remove(row);
                Destroy(row);
            }

            if (scoreRows.Count < ui_Main.players.Count)
            {
                UI_scoreRow row = Instantiate(scoreRowPrefab, transform).GetComponent<UI_scoreRow>();
                scoreRows.Add(row);
            }
        }

        int count = 0;

        foreach (UI_scoreRow row in scoreRows)
        {
            if (ui_Main.players[count].player is not Player)
            {
                row.letter.sprite = square;
                row.banner.sprite = square;
            }
            else
            {
                row.letter.sprite = ui_Main.players[count].player.playerClass.Leter;
                row.banner.sprite = ui_Main.players[count].player.playerClass.pattern;
            }

            row.banner.color = ui_Main.players[count].colour;

            row.userName.text = ui_Main.players[count].userName;
            row.kills.text = "" + ui_Main.players[count].kills;
            row.assits.text = "" + ui_Main.players[count].assists;
            row.deaths.text = ""+ui_Main.players[count].deaths;
            row.score.text = "" + ui_Main.players[count].GetScore();
            row.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 100);

            int even = 1;
            if (count % 2 == 0)
                even = -1;

            row.transform.eulerAngles = Vector3.up * 2.5f * even;
            count += 1;
        }

    } 
}
