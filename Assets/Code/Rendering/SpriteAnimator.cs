using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public float stepDuration = 0.25f;

    [Header("Sprites")]
    public SpriteDirections[] spriteAnimation;

    private DirectionalSprite directionalSprite;


    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();

        StartCoroutine(Step(0));
    }

    IEnumerator Step(int currentStep)
    {
        directionalSprite.constantFollow = true;
        directionalSprite.directionalSprites.Clear();
        directionalSprite.directionalSprites.AddRange(spriteAnimation[currentStep].sprites);
        directionalSprite.SetUp();


        if (currentStep < spriteAnimation.Length - 1)
        {
            yield return new WaitForSeconds(stepDuration);
            StartCoroutine(Step(currentStep + 1));
        }
    }
        /*
        public void Death()
        {
            timer += Time.deltaTime;
            if (animationPos < deathSprites.Length - 1)
            {
                if (timer > 0.25f)
                {
                    timer = 0;
                    animationPos += 1;
                }
            }

            directionalSprite.constantFollow = true;
            directionalSprite.directionalSprites.Clear();
            directionalSprite.directionalSprites.Add(deathSprites[animationPos]);
            directionalSprite.SetUp();
        } */
    }
