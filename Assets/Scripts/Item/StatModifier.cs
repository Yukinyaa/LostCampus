using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: enum 정리하기
public enum StatModType
{
    ATK,
    MaxHP,
    HPRegen,
    MaxStamina,
    StaminaRegen,
    MoveSpeed,
    JumpPower,
    Defence,
    MaxFullness
}

public class StatModifier : MonoBehaviour
{
    private StatModType statModType;

    public StatModType StatModType
    {
        get => statModType;
        set => statModType = value;
    }

    private float value;
    public float Value
    {
        get => value;
        set => this.value = value;
    }

    private Object source;
    public Object Source
    {
        get => source;
        set => source = value;
    }

}
