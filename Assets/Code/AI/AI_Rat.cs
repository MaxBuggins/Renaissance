using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WasteofMoney.Rosen;

public class AI_Rat : MonoBehaviour
{
    private NavMeshAgent navAgent;

    public RatHotSpot hotSpot;


    void Start()
    {
        navAgent = FindObjectOfType<NavMeshAgent>();

        if(hotSpot != null)
            navAgent.SetDestination(RandomNavSphere(hotSpot.transform.position, 1));
    }




    Vector3 FindDesternation()
    {
        Vector3 desternation = Vector3.zero;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (RatHotSpot spot in FindObjectsOfType<RatHotSpot>())
        {
            float dist = Vector3.Distance(spot.transform.position, currentPos);
            if (dist < minDist)
            {
                desternation = spot.transform.position;
                minDist = dist;
            }
        }
        
        return desternation;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask = -1, int attempts = 5)
    {
        Vector3 randDirection = Vector3.zero;
        NavMeshHit navHit;

        for (int i = attempts; i > 0; i--)
        {
            randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask) == true)
                return navHit.position;
        }

        return Vector3.zero;
    }
}
