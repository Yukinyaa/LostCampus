using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
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

    private bool isInvincible = false;
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private GameObject DamageIndicator;
    private TextMeshPro damageIndicatorText;
    private void Awake()
    {
        this.damageIndicatorText=DamageIndicator.GetComponent<TextMeshPro>();
    }

    public float GetATK()
    {
        
        this.PlayDamageIndicator(dam);
        PlayHitParticle(colPos);
        if (isInvincible) return;
        CmdGetDamaged(dam);
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
    private void PlayDamageIndicator(float dmg)
    {
        this.damageIndicatorText.text = dmg.ToString();
        this.DamageIndicator.transform.position = gameObject.transform.position;
        Instantiate(this.DamageIndicator);
    }
    private void PlayDamageIndicator(string dmg)
    {
        this.damageIndicatorText.text = dmg;
        this.DamageIndicator.transform.position = gameObject.transform.position;
        Instantiate(this.DamageIndicator);
    }
    public void PlayHitParticle(Vector3? pos = null)
    {
        Delay = true;
        yield return WaitForATKDelaySecond;
        Delay = false;
    }

    public void GetEliminated()
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

public class Status : MonoBehaviour
{
    public enum Team
    { 
        Friendly, Enemy, Neutral
    }
    #region SerializedFields
    [Tooltip("Health")]
    public float HP;
    [Tooltip("Attack power")]
    public float ATK;

    public float AP;

    public float MaxAP = 1000;

    public Team team;
    #endregion

    #region localValues
    
    #endregion

    
}