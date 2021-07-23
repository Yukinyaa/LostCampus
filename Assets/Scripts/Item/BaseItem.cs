using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[Serializable]
public class BaseItem : NetworkBehaviour
{
    private string instanceId;
    private ItemInfo _itemInfo;
    private int amount;
    
    
    public ItemInfo ItemInfo
    {
        get => _itemInfo;
    }
    
    public int Amount
    {
        get => amount;
        set => amount = value;
    }

    public BaseItem(ItemInfo itemInfo, int _amount = 1, string _instanceId = null)
    {
        _itemInfo = itemInfo;
        amount = _amount;
        instanceId = instanceId;
    }
}