using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ScaleOverTime : MonoBehaviour
{
    public Vector3 endScale;

    public float delay;
    public float duration;
    public AnimationCurve scaleCurve;

    void Start()
    {
        Tween.LocalScale(transform, endScale, duration, delay, scaleCurve);
    }
}
