using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLotto : NetworkBehaviour
{
    public ObjectPrize[] prizes;
    private float totalChance;

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
        float winingNumber = Random.Range(0, totalChance);
        RedeemPrize(winingNumber);
    }

    [Server]
    public void RedeemPrize(float winingNumber)
    {
        int index = 0;
        foreach (ObjectPrize prize in prizes)
        {
            winingNumber -= prize.chance;

            if (winingNumber <= 0)
            {
                RpcDisplayPrize(index);

                if (prize.health != 0)
                {
                    player.health += prize.health;
                    //prevents infiti health stacking
                    //if (player.health + prize.health > player.maxHealth)
                        //player.health = player.maxHealth;
                    //else
                        //player.health += prize.health;
                }

                if (prize.special != 0)
                    player.ServerAddSpecial(prize.special);


                return;
            }
            index++;
        }
    }

    [ClientRpc]
    public void RpcDisplayPrize(int index)
    {
        if(prizes[index].spawnInLocalSpace)
            Instantiate(prizes[index].spawnOnWin, transform.position + prizes[index].spawnOffset, Quaternion.identity, transform);
        else
            Instantiate(prizes[index].spawnOnWin, transform.position + prizes[index].spawnOffset, transform.rotation);
    }
}
