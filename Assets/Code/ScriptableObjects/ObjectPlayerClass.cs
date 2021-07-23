using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Class", menuName = "PlayerClass", order = 1)]
public class ObjectPlayerClass : ScriptableObject
{
    [Header("Player Movement")]
    public float speed = 5;
    public float backSpeedMultiplyer = 0.65f;
    public float sideSpeedMultiplyer = 0.8f;
    public float airMovementMultiplyer = 0.6f;

    public float gravitY = -10f; //manual gravity for gameplay reasons
    public float fricktion = 5f;
    public float maxMoveVelocity = 6;

    public float jumpHeight = 2f;
    public float coyotTime = 0.3f; //lol mesh has good idears NO WAY

    public float pushForce = 5f;
   
    [Header("Sprites")]
    public SpriteDirections[] idleSprites;
    public SpriteDirections[] runSprites;


    [Header("SoundEffects")]
    public AudioClip walkCycle;
    public AudioClip[] hurtSound;
    public AudioClip[] deathSound;
    public AudioClip[] jumpSound;

    public AudioClip[] hurtPlayerSound;

    public AudioClip[] spawnSound;
}
