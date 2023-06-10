using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hurtable : NetworkBehaviour
{
    [SyncVar] protected NetworkIdentity lastAttackerIdenity; //MUST BE BEFORE HEALTH because of sync order

    public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))] public int health = 100;



    [Server]
    public void Hurt(int damage, HurtType hurtType = HurtType.Death, NetworkIdentity attackerIdentity = null) //can be used to heal just do -damage
    {
        if (health <= 0)
            return;

        lastAttackerIdenity = attackerIdentity;

        //prevents infiti health stacking
        if (health - damage > maxHealth)
            health = maxHealth;
        else
            health -= damage;

        if (health <= 0)
        {
            if (attackerIdentity != null)
            {
                Player killer = attackerIdentity.GetComponent<Player>();
                if (killer != null)
                {
                    //killer.kills += 1;
                }
            }

            ServerDeath();
        }
    }


    void OnHealthChanged(int _Old, int _New)
    {
        if (_Old > _New) //if damage taken
        {
            int damage = _Old - _New;
            OnHurt(damage);

            if (_Old > 0 && _New <= 0) //on death
            {
                OnDeath();
            }
        }

        if (_Old < _New) //if heal taken
        {
            int heal = _Old - _New;
            OnHeal(heal);

            if (_Old <= 0 && _New > 0) //on Res
            {
                OnAlive();
            }
        }
    }

    public virtual void OnHurt(int damage) //do somethin or other to a toaster
    {

    }

    public virtual void OnHeal(int heal) //do somethin or other
    {

    }

    public virtual void OnDeath()
    {
        if (isLocalPlayer == false)
            Destroy(gameObject);
    }

    [ServerCallback]
    public virtual void ServerDeath()
    {
        NetworkServer.Destroy(gameObject);
        //DeathEvent.Invoke();
        //gameObject.SetActive(false);
        //Destroy(gameObject);
        //if(destoryOnDeath)
        //NetworkServer.Destroy(gameObject);
    }


    [ClientCallback]
    public virtual void OnAlive()
    {

    }

    [TargetRpc]
    public virtual void TargetAddVelocity(NetworkConnection target, Vector3 vel)
    {

    }

    [TargetRpc]
    public virtual void TargetSetVelocity(NetworkConnection target, Vector3 vel, bool ignorZero)
    {

    }
}
