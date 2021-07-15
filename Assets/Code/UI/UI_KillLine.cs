using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_KillLine : MonoBehaviour
{
    public float destroyDelay = 1.5f;

    public string killer;
    public string dier;
    public Sprite killSprite;

    public TextMeshProUGUI killerText;
    public TextMeshProUGUI dierText;
    public Image killImage;


    void Start()
    {
        Invoke(nameof(DestroySelf), destroyDelay); //so it goes away (Smelly)
    }

    public void UpdateInfo()
    {
        killerText.text = killer;
        dierText.text = dier;
        killImage.sprite = killSprite;
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
