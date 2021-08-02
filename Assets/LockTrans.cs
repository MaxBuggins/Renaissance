using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTrans : MonoBehaviour
{
    public Vector3 lockedRotation;
    public bool lockX = true;
    public bool lockY = true;
    public bool lockZ = true;

    void Start()
    {
        if (lockX && lockZ && lockY == false)
        {
            transform.eulerAngles = new Vector3(lockedRotation.x, transform.eulerAngles.y, lockedRotation.z);
        }
    }

}
