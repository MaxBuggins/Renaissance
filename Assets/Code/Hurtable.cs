using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hurtable : NetworkBehaviour
{
    [SyncVar] protected NetworkIdentity lastAttackerIdenity; //MUST BE BEFORE HEALTH because of sync order
    [SyncVar] protected NetworkIdentity lastHurtfulIdenity; //MUST BE BEFORE HEALTH because of sync order

    public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))] public int health = 100;

    [Server]
    public virtual void Hurt(int damage, HurtType hurtType = HurtType.Death, NetworkIdentity hurtfulIdentity = null, NetworkIdentity attackerIdentity = null)
    {
        //dont kill the dead, also this isn't the place for healing go to Heal()
        if (health <= 0 || damage < 0)
            return;

        lastAttackerIdenity = attackerIdentity;
        lastHurtfulIdenity = hurtfulIdentity;

        health -= damage;

        if (health <= 0)
        {
            if (attackerIdentity != null)
            {
                //Player killer = attackerIdentity.GetComponent<Player>();
                //if (killer != null)
                //{
                    //killer.kills += 1;
                //}
            }

            ServerDeath();
        }
    }

    [Server]
    public virtual void Heal(int damage, HurtType hurtType = HurtType.Death, NetworkIdentity attackerIdentity = null, NetworkIdentity hurtfulIdenity = null, bool canRevive = false)
    {
        if (damage < 0)
            return;

        if (health <= 0 && canRevive == false)
            return;

        lastAttackerIdenity = attackerIdentity;
        lastHurtfulIdenity = hurtfulIdenity;

        //prevents infiti health stacking
        if (health + damage > maxHealth)
            health = maxHealth;
        else
            health += damage;

       
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

    public virtual void OnHurt(int damage, Vector3 hitPos = default(Vector3), Vector3 hitRot = default(Vector3)) //do somethin or other to a toaster
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


    [Server]
    public virtual void ServerApplyEffect(StatusEffect.EffectType effect, float duration = Mathf.Infinity, float magnitude = 1)
    {
        ClientApplyEffect(effect, duration, magnitude);
        ApplyEffect(effect, duration, magnitude);
    }

    [ClientRpc]
    public virtual void ClientApplyEffect(StatusEffect.EffectType effect, float duration, float magnitude)
    {

    }

    public virtual void ApplyEffect(StatusEffect.EffectType effect, float duration = Mathf.Infinity, float magnitude = 1)
    {

    }
}
