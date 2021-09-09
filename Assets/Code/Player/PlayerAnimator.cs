using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

[System.Serializable]
public class SpriteDirections
{
    public Sprite[] sprites;
}
    
public class PlayerAnimator : MonoBehaviour
{
    private float timer = 0;
    private float sectionTime = 0;
    private int animationPos = 0;

    public float moveAmount = 0.25f;
    public Vector3 lastPos;

    [Header("Hurt")]
    public float hurtScale = 1.1f;
    public float hurtDuration = 0.5f;
    public AnimationCurve hurtCurve;

    [Header("Sprites")]
    public SpriteDirections[] idleSprites;
    public SpriteDirections[] runSprites;

    [Header("Materials")]
    public Material blank;
    public Material immunityBlank;

    private DirectionalSprite directionalSprite;
    private Player player;


    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {


        if (player.health > 0)
        {
            Move();
        }
        //else

    }

    private void Move()
    {
        directionalSprite.constantFollow = false;
        directionalSprite.directionalSprites = new List<Sprite>(runSprites[0].sprites);
    }

    void Hurt()
    {
        Tween.LocalScale(transform, transform.localScale * hurtScale, hurtDuration,
                0, hurtCurve);
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
