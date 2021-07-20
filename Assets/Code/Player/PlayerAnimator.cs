using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteDirections
{
    public Sprite[] sprites;
}
    
public class PlayerAnimator : MonoBehaviour
{
    
    public float moveAmount = 0.25f;
    public Vector3 lastPos;

    [Header("Sprites")]
    public SpriteDirections[] idleSprites;
    public SpriteDirections[] runSprites;

    private DirectionalSprite directionalSprite;


    void Start()
    {
        directionalSprite = GetComponent<DirectionalSprite>();
    }

    void Update()
    {
        
    }
}
