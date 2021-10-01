using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Status : NetworkBehaviour
{
    public enum Faction
    { 
        Friendly, Enemy, Neutral
    }
    #region Stat
    [SyncVar]
    public float Hp = 100;
    public float MaxHp = 100;
    public float Atk = 1;
    public float Ap = 5;
    public float MaxAp = 5;
    public Faction MyFaction;

    #endregion

    private bool isInvincible = false;
    [SerializeField] private ParticleSystem hitParticle;
    private ParticleManager ParticleManager;
    private void Awake()
    {
        ParticleManager=GameObject.Find("ParticleManager").GetComponent<ParticleManager>();
    }

    public void GetAttacked(float dam, Vector3? colPos = null)
    {
        
        this.ParticleManager.PlayDamageIndicator(this.gameObject,dam);
        PlayHitParticle(colPos);
        if (isInvincible) return;
        CmdGetDamaged(dam);
    }

    [Command(requiresAuthority = false)]
    public void CmdGetDamaged(float dam)
    {
        Hp = Hp - dam;
        if (Hp <= 0)
        {
            GetEliminated();
        }
    }
    public void PlayHitParticle(Vector3? pos = null)
    {
        if (hitParticle == null) return;
        if(pos !=null)
            hitParticle.transform.position = (Vector3) pos;
        hitParticle.Play();
    }

    public void GetEliminated()
    {
        NetworkServer.Destroy(gameObject);
    }

}