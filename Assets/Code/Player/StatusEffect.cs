using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StatusEffect : MonoBehaviour
{
    public enum EffectType {immunity, sneeze, }
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

        if (effectType == EffectType.immunity)
            player.body.GetComponent<MeshRenderer>().material = player.playerAnimator.immunityBlank;
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
                    player.body.GetComponent<MeshRenderer>().material = player.playerAnimator.blank;
                    break;
                }

            case (EffectType.sneeze):
                {
                    audioSource.PlayOneShot(player.playerClass.sneeze[Random.Range(0, player.playerClass.sneeze.Length)]);
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
