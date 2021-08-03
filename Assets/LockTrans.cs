using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTrans : MonoBehaviour
{
    public Vector3 lockedRotation;
    public bool lockX = true;
    public bool lockY = true;
    public bool lockZ = true;

    public float fallToFloorSpeed = 1; //if 0 then dont fall
    private Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;

        if (lockX && lockZ && lockY == false)
        {
            transform.eulerAngles = new Vector3(lockedRotation.x, transform.eulerAngles.y, lockedRotation.z);
        }
    }

    private void Update()
    {
        transform.position += -transform.up * fallToFloorSpeed * Time.deltaTime;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.2f, transform.position - lastPos,
            out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f,
            3, QueryTriggerInteraction.Ignore))

            fallToFloorSpeed = 0;


        if (fallToFloorSpeed <= 0)
            enabled = false; //stop running so more efficentcy


        lastPos = transform.position;
    }

}
