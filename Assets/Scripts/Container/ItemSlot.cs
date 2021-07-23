using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ItemSlot
{
    private ItemInfo _itemInfo;
    public ItemInfo ItemInfo
    {
        get => _itemInfo;
    }

    private int amount;
    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            OnValueChanged(this);
        }
    }

    private UnityAction<ItemSlot> _OnValueChanged;

    public UnityAction<ItemSlot> OnValueChanged
    {
        get => _OnValueChanged;
        set => _OnValueChanged = value;
    }

    public bool isFull()
    {
        return Amount >= _itemInfo.maxStack;
    }


    public ItemSlot()
    {
        
    }
    
    public ItemSlot(ItemInfo itemInfo, int _amount = 1)
    {
        _itemInfo = itemInfo;
        amount = _amount;
    }
}