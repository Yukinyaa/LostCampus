using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
/*
public class Status : MonoBehaviour
{
    #region SerializedFields
    [Tooltip("Health")]
    public float HP;
    [Tooltip("Attack power")]
    public float ATK;
    #endregion

    #region localValues
    private float ATKDelaySecond = 1f;
    private WaitForSeconds WaitForATKDelaySecond;
    private bool Delay = false;
    #endregion

    private void Awake()
    {
        WaitForATKDelaySecond = new WaitForSeconds(this.ATKDelaySecond);
    }

    public float GetATK()
    {
        return this.ATK;
    }

    public float TotalATK()
    {
        float ATK = 0;
        if (!this.Delay)
        {
            ATK = this.ATK;
            StartCoroutine(ATKIEnumerator());
        }
        return ATK;
    }

    private IEnumerator ATKIEnumerator()
    {
        Delay = true;
        yield return WaitForATKDelaySecond;
        Delay = false;
    }

    public float GetHP()
    {
        return this.HP;
    }

    public float ApplyATK(float ATK)
    {
        this.HP = this.HP > ATK ? this.HP - ATK : this.HP = 0;
        return this.HP;
    }
}
*/

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

    #region localValues

    private bool isInvincible = false;
    [SerializeField] private ParticleSystem hitParticle;
    #endregion

    public void GetAttacked(float dam, Vector3? colPos = null)
    {
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