using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLotto : NetworkBehaviour
{
    public ObjectPrize[] prizes;
    private float totalChance;

    public float prizeRedeemDelay = 0.4f;

    private ObjectPrize currentPrize;

    public Player player;


    private void Start()
    {
        foreach (ObjectPrize prize in prizes)
        {
            totalChance += prize.chance;
        }
    }


    [Command]
    public void CmdEnterLotto()
    {
        float winingNumber = UnityEngine.Random.Range(0, totalChance);
        RedeemPrize(winingNumber);
    }

    [Server]
    private void RedeemPrize(float winingNumber)
    {
        int index = 0;
        foreach (ObjectPrize prize in prizes)
        {
            winingNumber -= prize.chance;

            if (winingNumber <= 0)
            {
                bool isUseless = (player.health >= player.maxHealth && prize.health > 0);//Health prize is usless if at full health

                if (isUseless == false)
                {
                    TargetDisplayPrize(netIdentity.connectionToClient ,index);

                    currentPrize = prize;
                    Invoke(nameof(ActivatePrize), prizeRedeemDelay);
                    return;
                }
            }
            index++;
        }
    }

    [Server]
    private void ActivatePrize()
    {
        if (currentPrize.networkSpawnObject != null)
        {
            GameObject spawned = Instantiate(currentPrize.networkSpawnObject, transform.position, transform.rotation, null);

            Hurtful hurt = spawned.GetComponentInChildren<Hurtful>();
            if (hurt != null)
            {
                hurt.owner = player;
                hurt.ownerID = netIdentity.netId;
            }

            NetworkServer.Spawn(spawned);
        }

        if (currentPrize.health != 0)
        {
            player.Hurt(-currentPrize.health, HurtType.Gambling, netIdentity);
        }

        if (currentPrize.score > 0)
        {
            player.addScore(currentPrize.score);
        }

        if (currentPrize.effect != StatusEffect.EffectType.none)
        {
            player.ApplyEffect(currentPrize.effect, currentPrize.duration, currentPrize.magnitude);
        }
  

        if (currentPrize.special != 0)
            player.ServerAddSpecial(currentPrize.special);
    }

    [TargetRpc]
    public void TargetDisplayPrize(NetworkConnection target, int index)
    {
        UI_Main.instance.ScratchLottoTicket(prizes[index].name, prizes[index].prizeSprite);

        if (prizes[index].spawnOnWin != null)
        {
            StartCoroutine(InstergateWinObject(index));
        }
    }

    IEnumerator InstergateWinObject(int index)
    {
        yield return new WaitForSeconds(prizeRedeemDelay - 0.1f);

        if (prizes[index].spawnInLocalSpace)
            Instantiate(prizes[index].spawnOnWin, transform.position + prizes[index].spawnOffset, Quaternion.identity, transform);
        else
            Instantiate(prizes[index].spawnOnWin, transform.position + prizes[index].spawnOffset, transform.rotation);
    }
}
