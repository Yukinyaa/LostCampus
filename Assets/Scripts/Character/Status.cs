using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float MaxHP = 100;
    public float MaxAP = 1000;

    public Team team;
    #endregion

    #region localValues
    
    #endregion

    
}