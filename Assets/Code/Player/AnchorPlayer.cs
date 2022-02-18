using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPlayer : MonoBehaviour
{
    public float radius = 5f;
    public float maxRadius = 7f;
    public float anchorForce = 5;

    public Player player;
    Hurtful hurtful;


    void Start()
    {
        hurtful = GetComponent<Hurtful>();
        player = hurtful.owner;

        player.GetComponentInChildren<FireFighterWeapon>().fireHydrant = this.GetComponent<ClientActivated>(); //SPEGETTI
    }

    void Update()
    {
        Vector3 distance = (transform.position - player.transform.position);
        if(distance.magnitude > radius)
        {
            Vector3 force = distance;

            player.velocity += force * anchorForce * Time.deltaTime;
        }

        if (distance.magnitude > maxRadius)
        {
            Vector3 force = distance;

            player.velocity += force * anchorForce * Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color colour = new Color(1, 0.5f, 0, 0.1f);
        Gizmos.color = colour;

        Gizmos.DrawSphere(transform.position, radius);

        colour = new Color(1, 1, 0, 0.2f);
        Gizmos.color = colour;

        Gizmos.DrawSphere(transform.position, maxRadius);
    }
}
