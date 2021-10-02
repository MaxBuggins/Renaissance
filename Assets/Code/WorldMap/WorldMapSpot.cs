using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapSpot : MonoBehaviour
{
    public bool Level = true;
    public string sceneName;

    public WorldMapSpot[] NeighbourSpots; //clockwise up-right-down-left

    //public SpriteRenderer spriteRender;

    void Start()
    {
        //spriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        int spotNum = 0;
        foreach (WorldMapSpot spot in NeighbourSpots)
        {

            if (NeighbourSpots[spotNum] != null)
                Gizmos.DrawLine(transform.position, NeighbourSpots[spotNum].transform.position);

            spotNum++;
        }
    }
}
