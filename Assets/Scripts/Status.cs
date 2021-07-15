using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float HP;
    public float ATK;

    private float DamageDelaySecond = 1f;

    private WaitForSeconds DamageDelay;

    private bool Delay = false;
    private void Awake()
    {
        DamageDelay = new WaitForSeconds(this.DamageDelaySecond);
    }

    public float GetDamage()
    {
        return this.ATK;
    }

    public float SetDamage(float ATK)
    {
        this.ATK = ATK;
        return this.ATK;
    }

    public float AmountOfDamage()
    {
        float ATK = 0;
        if (!this.Delay)
        {
            ATK = this.ATK;
            StartCoroutine(DamageIEnumerator());
        }
        return ATK;
    }

    private IEnumerator DamageIEnumerator()
    {
        Delay = true;
        yield return DamageDelay;
        Delay = false;
    }

    public float GetHealth()
    {
        return this.HP;
    }

    public float SetHealth(float HP)
    {
        this.HP = HP;
        return this.HP;
    }

    public float ApplyDamage(float Damage)
    {
        this.HP = this.HP > Damage ? this.HP - Damage : this.HP = 0;
        return this.HP;
    }
}
