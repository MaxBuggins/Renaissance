using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePhysics : MonoBehaviour
{
    public float segmentDistance = 1.5f;
    private float lineLength = 2;

    private Hurtful hurtful;
    public Transform endPos;

    private LineRenderer lineRender;

    void Start()
    {
        lineRender = GetComponent<LineRenderer>();

        if (lineRender == null)
        {
            enabled = false;
            return;
        }

        //very hard coded and bad but no idear how to do any other way
        hurtful = GetComponentInParent<Hurtful>();

        endPos = hurtful.owner.transform;


        //Invoke(nameof(AddSegment), segmentDelay);
    }


    void Update()
    {
        lineRender.SetPosition(0, endPos.position);
        lineRender.SetPosition(lineRender.positionCount - 1, transform.position);

        if(Mathf.Abs(Vector3.Distance(lineRender.GetPosition(lineRender.positionCount - 1),
            lineRender.GetPosition(lineRender.positionCount - 2)))
            > segmentDistance)
        {
            AddSegment();
        }
    }

    void AddSegment()
    {
        lineRender.positionCount += 1;
        lineRender.SetPosition(lineRender.positionCount - 1, transform.position);

        //Invoke(nameof(AddSegment), segmentDelay);
    }
}
