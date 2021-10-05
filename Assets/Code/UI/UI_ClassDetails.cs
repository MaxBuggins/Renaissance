using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_ClassDetails : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI noise;
    public Image health;
    public Image special;
    public Image speed;
    public Image primary;
    public TextMeshProUGUI discription;
    public TextMeshProUGUI lore;

    public void DisplayDetails(ObjectPlayerClass playerClass)
    {
        title.text = playerClass.name;
        noise.text = playerClass.noise;
        discription.text = playerClass.discription;
        health.fillAmount = (float)playerClass.maxHealth / 130;
        speed.fillAmount = playerClass.movementRateing / 10;
        special.fillAmount = playerClass.specialRateing / 10;
        primary.fillAmount = playerClass.primaryRateing / 10;
        lore.text = playerClass.lore;
    }
}
