using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using Mirror;

[System.Serializable]
public class SpriteDirections
{
    public Sprite[] sprites;
}
    
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private float timer = 0;
    private float sectionTime = 0;
    private int animationPos = 0;

    public float moveAmount = 0.25f;
    public float speedMultiplyer = 4;

    public float rotateAmount = 0.5f;

    [Header("Hurt")]
    public float hurtScale = 1.1f;
    public float hurtDuration = 0.5f;
    public AnimationCurve hurtCurve;

    //[Header("Sprites")]
    //public SpriteDirections[] idleSprites;
    //public SpriteDirections[] runSprites;


    [Header("Materials")]
    public Material blank;
    public Material immunityBlank;

    //private DirectionalSprite directionalSprite;
    private Player player;
    [HideInInspector] public Animator animator;


    void Start()
    {
        //directionalSprite = GetComponent<DirectionalSprite>();
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();

        if (!player.isLocalPlayer)
        {
            enabled = false;
        }
    }


    private void FixedUpdate()
    {
        if (player.health > 0)
        {
            if (player.velocity.magnitude > moveAmount)
            {
                animator.SetFloat("Moveing", player.velocity.magnitude * speedMultiplyer * Time.fixedDeltaTime);
            }
            else
            {
                animator.SetFloat("Moveing", 0);
                //animator.speed = 1;
                //animator.speed = player.lastYRot * speedMultiplyer * Time.fixedDeltaTime;
            }

            if (player.canStand == false)
            {
                animator.SetBool("Falling", true);
            }
            else
                animator.SetBool("Falling", player.fallTime > 0.1f);
        }

    }

    public void primaryAttack()
    {
        animator.SetTrigger("Primary");
    }

    public void secondaryAttack()
    {
        animator.SetTrigger("Secondary");
    }

    public void Special()
    {
        animator.SetTrigger("Special");
        //Tween.LocalScale(transform, transform.localScale * hurtScale, hurtDuration,
        //0, hurtCurve);
    }

    public void Hurt()
    {
        animator.SetTrigger("Hurt");
        //Tween.LocalScale(transform, transform.localScale * hurtScale, hurtDuration,
                //0, hurtCurve);
    }

    public void Dance()
    {
        animator.SetTrigger("Dance");
    }
}
