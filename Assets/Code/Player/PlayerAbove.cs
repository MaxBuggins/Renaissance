using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAbove : MonoBehaviour
{
    public int reactDuration = 3;

    public Light topLight;
    public TextMeshPro playerNameText;

    public GameObject currentReaction;
    public GameObject[] reactions;

    void Awake()
    {
        topLight = GetComponentInChildren<Light>();
        playerNameText = GetComponentInChildren<TextMeshPro>();
    }

    public void StartReaction(int index)
    {
        if (currentReaction != null || index >= reactions.Length) //one reaction at a time
            return;

        currentReaction = Instantiate(reactions[index], transform);

        Invoke(nameof(EndReaction), reactDuration);
    }

    public void EndReaction()
    {
        Destroy(currentReaction);
        currentReaction = null;
    }
}
