using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
