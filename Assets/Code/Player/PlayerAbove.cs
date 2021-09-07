using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAbove : MonoBehaviour
{
    public float reactDuration = 3;

    public Light topLight;
    public TextMeshPro playerNameText;

    public GameObject currentReaction;
    public GameObject[] reactions;

    private Player player;

    void Awake()
    {
        topLight = GetComponentInChildren<Light>();
        playerNameText = GetComponentInChildren<TextMeshPro>();
        player = GetComponentInParent<Player>();
    }

    public void StartReaction(int index)
    {
        if (currentReaction != null || index >= reactions.Length) //one reaction at a time
            return;

        currentReaction = Instantiate(reactions[index], transform);

        ParticleSystem.MainModule settings = currentReaction.GetComponent<ParticleSystem>().main;

        if (settings.startColor.color != Color.white)
            settings.startColor = new ParticleSystem.MinMaxGradient(player.playerColour);

        Invoke(nameof(EndReaction), reactDuration);
    }

    public void EndReaction()
    {
        //Destroy(currentReaction);
        currentReaction = null;
    }
}
