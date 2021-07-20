using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatHotSpot : MonoBehaviour
{
    public Vector2Int ratCountRange = new Vector2Int(5, 10);


    public GameObject ratObj;


    void Awake()
    {
        int ratCount = Random.Range(ratCountRange.x, ratCountRange.y);

        for(int i = 0; i < ratCount; i++)
        {
            GameObject rat = Instantiate(ratObj, transform.position, Quaternion.Euler(0,0,0));
            ratObj.GetComponent<AI_Rat>().hotSpot = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
