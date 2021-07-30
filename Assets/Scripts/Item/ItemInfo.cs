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
    public Dictionary<StatModType, int> stats = new Dictionary<StatModType, int>();
    [Range(1, 99999)]
    public int maxStack;
    public string sprite;
    
    public ItemInfo(int _id, string _name, int _rarity, ItemType _type, string _flavorText, 
        Dictionary<StatModType, int> _stats, int _maxStack, string _sprite)
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

    public int ID
    {
        get => id;
        set => id = value;
    }

    public string Name
    {
        get => name;
        set => name = value;
    }

    public int Rarity
    {
        get => rarity;
        set => rarity = value;
    }

    public ItemType Type
    {
        get => type;
        set => type = value;
    }

    public string FlavorText
    {
        get => flavorText;
        set => flavorText = value;
    }

    public Dictionary<StatModType, int> Stats
    {
        get => stats;
        set => stats = value;
    }

    public int MAXStack
    {
        get => maxStack;
        set => maxStack = value;
    }

    public string Sprite
    {
        get => sprite;
        set => sprite = value;
    }
}