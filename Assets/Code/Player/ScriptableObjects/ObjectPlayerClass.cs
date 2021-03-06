using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerClass { Artist, Banker, Convict, Fireman, Maid, Vet }

[CreateAssetMenu(fileName = "Player Class", menuName = "PlayerClass", order = 1)]
public class ObjectPlayerClass : ScriptableObject
{
    [Header("Bluff Text")]
    public string noise = "Ugh";
    public string discription;
    public string lore;
    [Range(0, 10)] public float specialRateing = 5;
    [Range(0, 10)] public float primaryRateing = 7;
    [Range(0, 10)] public float movementRateing = 8;
    public Sprite classSprite;
    public Sprite Leter;
    public Color32 color;
    public Sprite pattern;

    [Header("Player Stats")]
    public PlayerClass playerClass = PlayerClass.Convict;
    public bool upperCase = false;
    public int maxHealth = 100;
    public int maxSpecial = 10;

    public int spawnSpecial = 4;
    public float specialChargeRate = 6;

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
    public float slideFriction = 0.3f;

    [Header("Other")]
    public bool convertHealingToSpecial = false;


    [Header("Player Decor")]
    public int bloodPerDamage = 6;
    public GameObject bloodObj;


    [Header("SoundEffects")]
    //public AudioClip walkCycle;
    public AudioClip[] hurtSound;
    public AudioClip[] deathSound;
    public AudioClip[] jumpSound;

    public AudioClip[] hurtPlayerSound;
    public AudioClip[] killPlayerSound;

    public AudioClip[] spawnSound;

    public AudioClip[] sneeze;
}
