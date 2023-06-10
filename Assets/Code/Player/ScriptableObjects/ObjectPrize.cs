using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPrize", menuName = "Prize", order = 2)]
public class ObjectPrize : ScriptableObject
{
    public Sprite prizeSprite;

    public float chance; //higher the more chance (relative to other prizes)

    [Header("Winnings")]
    public int health;
    public int special;
    public int score;

    [Header("Status Effect")]
    public StatusEffect.EffectType effect;
    public float magnitude = 1;
    public float duration = 3;

    [Header("SpawnObject")]
    public GameObject networkSpawnObject;

    [Header("Decor")]
    public GameObject spawnOnWin;
    public bool spawnInLocalSpace = false;
    public Vector3 spawnOffset;

}
