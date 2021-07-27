using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePhysics : MonoBehaviour
{
    public float segmentDistance = 1.5f;
    private float lineLength = 2;

    public Transform endPos;

    private LineRenderer lineRender;

    void Start()
    {
        lineRender = GetComponent<LineRenderer>();

        //very temp
        
        
        endPos = GetComponentInParent<Hurtful>().owner.transform;

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
