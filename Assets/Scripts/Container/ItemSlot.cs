using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ItemSlot
{
    private int infoID = -1;

    [NonSerialized] private ItemInfo _itemInfo;
    public ItemInfo itemInfo
    {
        get
        {
            if (_itemInfo == null)
            {
                _itemInfo = ItemInfoDataBase.FindItemInfo(infoID);
            }
            return _itemInfo;
        }
    }

    private string sprite; //

    public int InfoID
    {
        get => infoID;
    }
    public string Name
    {
        get => itemInfo.name;
    }
    public int Rarity
    {
        get => itemInfo.rarity;
    }
    public ItemType Type
    {
        get => itemInfo.type;
    }
    public string FlavorText
    {
        get => itemInfo.flavorText;
    }
    public int MAXStack
    {
        get => itemInfo.maxStack;
    }

    public Dictionary<StatModType, int> Stats
    {
        get => itemInfo.stats;
        set => itemInfo.stats = value;
    }
    public string Sprite
    {
        get => sprite;
        set => sprite = value;
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
        sprite = _itemInfo.sprite;
        amount = _amount;
    }
}