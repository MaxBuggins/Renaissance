using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CapturePoint : NetworkBehaviour
{
    public float pointDelay = 0.5f;
    private float pointTime;

    [SyncVar(hook = nameof(OnColourChange))]
    public Color32 currentColour = Color.white;

    public List<Player> playersIn = new List<Player>();

    [Header("Unity Stuff")]
    private Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();
        OnColourChange(Color.white, currentColour);
    }

    void OnColourChange(Color32 _Old, Color32 _New)
    {
        render.material.color = _New;
    }

    [Server]
    void Update()
    {
        pointTime += Time.deltaTime;

        if(pointTime > pointDelay)
        {
            Color32[] colours = new Color32[playersIn.Count];
            int i = 0;
            foreach (Player player in playersIn)
            {
                if (player.health > 0)
                    player.addScore(1);
                else
                    playersIn.Remove(player);

                colours[i] = player.playerStats.colour;
                i++;
            }


            currentColour = CombineColors(colours);

            pointTime = 0;
        }
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            playersIn.Add(player);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            playersIn.Remove(player);
        }
    }

    public static Color CombineColors(params Color32[] aColors)
    {
        if(aColors.Length == 0)
        {
            return (Color.white);
        }

        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in aColors)
        {
            result += c;
        }
        result /= aColors.Length;
        return result;
    }
}
