using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour
{
    private int intanceId;
    private ItemInfo itemInfo;
    private int amount;
    
    public int IntanceId
    {
        get => intanceId;
    } 
    
    public ItemInfo ItemInfo
    {
        get => itemInfo;
    }
    
    public int Amount
    {
        get => amount;
        set => amount = value;
    } 
    
    public Item(ItemInfo _itemInfo, int _amount = 1)
    {
        itemInfo = itemInfo;
        amount = _amount;
    }
}
