using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class NetworkTween : MonoBehaviour
{
    private Vector3 startValue;
    public Vector3 endValue;

    public float delay = 0;
    public float duration = 3;

    public AnimationCurve animationCurve = Tween.EaseWobble;

    private float totalTime; //delay + duration


    void Start()
    {
        startValue = transform.position;

        totalTime = delay + duration;

        float timeOffset = (float)NetworkTime.time;

        if (timeOffset > 0)
        {
            timeOffset -= totalTime;
        }

        timeOffset = -timeOffset;

        //float relativeOffset = totalTime / timeOffset;

        float catchUpDelay = delay - timeOffset;
        float catchUpDuration = duration;

        if (catchUpDelay < 0)
        {
            catchUpDuration += catchUpDelay;
        }

        catchUpDelay = Mathf.Clamp(catchUpDelay, 0, delay);


        Tween.Position(transform, endValue, catchUpDuration, catchUpDelay, animationCurve, Tween.LoopType.None, OnTweenStart, OnTweenFinish);
    }

    public void OnTweenStart()
    {
        //Tween.Position(transform, endValue, delay, duration, animationCurve, Tween.LoopType.Loop, OnTweenStart, OnTweenFinish, false);
    }

    public void OnTweenFinish()
    {
        Tween.Position(transform, startValue, endValue, duration, delay, animationCurve, Tween.LoopType.None, OnTweenStart, OnTweenFinish, false);
    }


    private void OnDrawGizmos() //Looks cool in scene view
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, endValue);

    }

}
