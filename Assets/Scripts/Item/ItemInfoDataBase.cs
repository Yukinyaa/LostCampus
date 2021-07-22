using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: enum 정리하기
[Serializable]
public enum ItemType
{
    WEAPON,
    INGRIDIENT,
    ARMOR
}

[Serializable]
public struct ItemInfo
{
    public int id;
    public string name;
    public int rarity;
    public ItemType type;
    public string flavorText;
    public Dictionary<string, int> stats;
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


public static class ItemInfoDataBase
{
   private static List<ItemInfo> itemInfos;
   public static List<ItemInfo> ItemInfos 
   {
       get
       {
           if (itemInfos == null)
           {
               LoadItemInfoFromDB("");
           }

           return itemInfos;
       }
   }

   public static ItemInfo FindItemInfo(int id)
   {
       return itemInfos.Find(x => x.id == id);
   }
   
    //TODO: 기획 정해지고 csv 파싱하기
    public static void LoadItemInfoFromDB(string path)
    {
        
    }
}
