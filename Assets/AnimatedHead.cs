using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class AnimatedHead : MonoBehaviour
{
    public enum eyeState { closed, lookLeft, lookForward, lookRight };

    [Header("FaceValues")]
    public float blinkRate = 2;
    private float timeSinceBlink = 0;

    [Header("Internal")]
    public eyeState currentEyeState = 0;
    private bool courutineIsRunning = false;

    [Header("Refrences")]
    [SerializeField] private SpriteRenderer rightEye;
    [SerializeField] private SpriteRenderer leftEye;
    [SerializeField] private SpriteRenderer mouth;

    [Header("Sprites")]
    [SerializeField] private Sprite[] eyeSprites;
    [SerializeField] private Sprite[] mouthSprites;

    void Start()
    {
        
    }


    void Update()
    {
        UpdateEyes();
        return;

        if (courutineIsRunning)
            return;

        LookAround();
        return;

        timeSinceBlink += Time.deltaTime;
        if(timeSinceBlink > blinkRate)
        {
            timeSinceBlink = 0;
            Blink();

        }
    }

    public void UpdateEyes()
    {
        switch(currentEyeState)
        {
            case eyeState.closed:
                {
                    leftEye.sprite = eyeSprites[2];
                    rightEye.sprite = eyeSprites[2];
                    break;
                }
            case eyeState.lookLeft:
                {
                    leftEye.sprite = eyeSprites[4];
                    rightEye.sprite = eyeSprites[0];
                    break;
                }
            case eyeState.lookForward:
                {
                    leftEye.sprite = eyeSprites[3];
                    rightEye.sprite = eyeSprites[3];
                    break;
                }
            case eyeState.lookRight:
                {
                    leftEye.sprite = eyeSprites[0];
                    rightEye.sprite = eyeSprites[4];
                    break;
                }
        }
    }


    public void Blink()
    {
        StartCoroutine(BlinkSequence());
    }

    IEnumerator BlinkSequence()
    {
        courutineIsRunning = true;

        leftEye.sprite = eyeSprites[1];
        rightEye.sprite = eyeSprites[1];
        yield return new WaitForSeconds(0.03f);

        leftEye.sprite = eyeSprites[2];
        rightEye.sprite = eyeSprites[2];
        yield return new WaitForSeconds(0.12f);

        leftEye.sprite = eyeSprites[1];
        rightEye.sprite = eyeSprites[1];
        yield return new WaitForSeconds(0.04f);

        leftEye.sprite = eyeSprites[0];
        rightEye.sprite = eyeSprites[0];

        courutineIsRunning = false;
    }

    public void LookAround()
    {
        StartCoroutine(LookAroundSequence());
    }

    IEnumerator LookAroundSequence()
    {
        courutineIsRunning = true;

        leftEye.sprite = eyeSprites[0];
        rightEye.sprite = eyeSprites[3];
        yield return new WaitForSeconds(0.4f);

        leftEye.sprite = eyeSprites[1];
        rightEye.sprite = eyeSprites[1];
        yield return new WaitForSeconds(0.03f);

        leftEye.sprite = eyeSprites[2];
        rightEye.sprite = eyeSprites[2];
        yield return new WaitForSeconds(0.12f);

        leftEye.sprite = eyeSprites[1];
        rightEye.sprite = eyeSprites[1];
        yield return new WaitForSeconds(0.03f);

        leftEye.sprite = eyeSprites[3];
        rightEye.sprite = eyeSprites[0];
        yield return new WaitForSeconds(0.4f);

        courutineIsRunning = false;
    }

}
