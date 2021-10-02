using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapCamera : MonoBehaviour
{
    public float cameraFollowSpeed = 3;

    public float distanceFromPlayer = 25;

    private WorldMapPlayer player;
    public Transform centerPos; 

    void Start()
    {
        player = FindObjectOfType<WorldMapPlayer>();

        //centerPos.eulerAngles = new Vector3(Mathf.Asin(player.transform.position.z / distanceFromCenter),
            //Mathf.Asin(player.transform.position.y / distanceFromCenter),
            //Mathf.Asin(player.transform.position.x / distanceFromCenter));

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, distanceFromPlayer);
    }

    void Update()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * cameraFollowSpeed),
        Mathf.Lerp(transform.position.y, player.transform.position.y, Time.deltaTime * cameraFollowSpeed), distanceFromPlayer);

       //centerPos.eulerAngles = new Vector3(Mathf.Asin(player.transform.position.z / distanceFromCenter),
            //Mathf.Asin(player.transform.position.y / distanceFromCenter),
            //Mathf.Asin(player.transform.position.x / distanceFromCenter)) * 360;


    }
}
