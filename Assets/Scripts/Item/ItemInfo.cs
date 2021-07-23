using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// 에셋메뉴로 만들게 해둠
/// </summary>
[CreateAssetMenu(fileName = "ItemInfo", menuName = "Item/New ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public int id = -1;
    public string name;
    public int rarity;
    public ItemType type;
    public string flavorText;
    public Dictionary<string, int> stats;
    [Range(1, 99999)]
    public int maxStack;
    public string sprite;
    
    public ItemInfo(int _id, string _name, int _rarity, ItemType _type, string _flavorText, 
        Dictionary<string, int> _stats, int _maxStack, string _sprite)
    {
        id = _id;
        name = _name;
        rarity = _rarity;
        type = _type;
        flavorText = _flavorText;
        stats = _stats;
        maxStack = _maxStack;
        sprite = _sprite;
    }
}