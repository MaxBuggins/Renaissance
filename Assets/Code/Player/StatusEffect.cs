using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pixelplacement;

public class StatusEffect : MonoBehaviour
{
    public enum EffectType {none, immunity, sneeze, speed, all}
    public EffectType effectType;

    public float duration = Mathf.Infinity;
    public float magnitude;

    private float flashTime;

    public GameObject[] particalEffects; //how to use

    private Player player;
    private AudioSource audioSource;

    private void Start()
    {
        player = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();

        //if (effectType == EffectType.immunity)
        //player.body.material = player.playerAnimator.immunityBlank;

        switch (effectType)
        {
            case (EffectType.speed):
                {
                    Tween.Value(player.speed, player.speed * magnitude, HandleSpeedChange, 0.5f, 0);
                    player.maxMoveVelocity *= magnitude / 2;

                    if(player.isLocalPlayer == true)
                    {
                        Tween.Value(Camera.main.fieldOfView, Camera.main.fieldOfView + magnitude * 10, HandleFOVChange, 1f, 0);
                    }

                    break;
                }
            case (EffectType.all):
                {
                    player.speed += magnitude;
                    player.maxMoveVelocity *= magnitude / 2;
                    player.jumpHeight += magnitude;
                    player.gravitY += magnitude;
                    player.fricktion += magnitude;
                    player.specialChargeRate += magnitude;
                    player.coyotTime += (magnitude / 10);
                break;
                }
        }
    }

    void HandleFOVChange(float value)
    {
        Camera.main.fieldOfView = value;
    }

    void HandleSpeedChange(float value)
    {
        player.speed = value;
    }

    public void Update()
    {

        duration -= Time.deltaTime;

        if (duration < 0) //end of duration do client and server end effects
        {
            if (player.netIdentity.isClient)
                ClientEndEffect();
            if(player.netIdentity.isServer)
                StartCoroutine(ServerEndEffect()); //case for host

            enabled = false;
        }
    }

    [Server]
    IEnumerator ServerEndEffect()
    {
        yield return new WaitForEndOfFrame();

        switch (effectType)
        {
            case (EffectType.sneeze):
                {
                    yield return new WaitForSeconds(0.4f);
                    player.TargetSetVelocity(player.connectionToClient, -transform.forward * magnitude + transform.up * magnitude * 0.3f);
                    break;
                }
        }

        removeEffect();
    }

    [Client]
    public void ClientEndEffect()
    {
        switch (effectType)
        {
            case (EffectType.immunity):
                {
                    //player.body.material = player.playerAnimator.blank;
                    break;
                }

            case (EffectType.sneeze):
                {
                    audioSource.PlayOneShot(player.playerClass.sneeze[Random.Range(0, player.playerClass.sneeze.Length)]);
                    break;
                }

            case (EffectType.speed):
                {
                    player.speed = player.playerClass.speed;
                    player.maxMoveVelocity = player.playerClass.maxMoveVelocity;

                    Camera.main.fieldOfView = 95;
                    break;
                }
            case (EffectType.all):
                {
                    player.speed = player.playerClass.speed;
                    player.maxMoveVelocity = player.playerClass.maxMoveVelocity;
                    player.jumpHeight -= magnitude;
                    player.gravitY -= magnitude;
                    player.fricktion -= magnitude;
                    player.specialChargeRate -= magnitude;
                    player.coyotTime -= (magnitude / 10);
                    break;

                }
        }

        if (!player.netIdentity.isServer) //in case this is a host, wait for server stuff
            removeEffect();
    }

    void removeEffect()
    {
        player.statusEffects.Remove(this);
        Destroy(this);
    }
}
