using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTrans : MonoBehaviour
{
    public Vector3 lockedRotation;
    public bool lockX = true;
    public bool lockY = true;
    public bool lockZ = true;

    public float fallToFloorSpeed = 1; //if 0 then dont fall (obvious why did i write this)
    public float radius = 0.2f;
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
        if (Physics.SphereCast(lastPos, radius, transform.position - lastPos,
            out hit, maxDistance: Mathf.Abs(Vector3.Distance(transform.position, lastPos)) * 1.25f,
            3, QueryTriggerInteraction.Ignore))

            enabled = false;

        lastPos = transform.position;
    }
}
