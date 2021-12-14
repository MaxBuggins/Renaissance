using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PlayerOutline : MonoBehaviour
{
    private PlayerStats playerStats;
    private Outline outline;

    void OnEnable()
    {
        outline = GetComponent<Outline>();
        playerStats = GetComponentInParent<PlayerStats>();
        outline.OutlineColor = playerStats.colour;
    }
}
