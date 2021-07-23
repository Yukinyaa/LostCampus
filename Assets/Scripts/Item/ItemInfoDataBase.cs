#define TEST_ITEM

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//TODO: enum 정리하기
[Serializable]
public enum ItemType
{
    WEAPON,
    INGRIDIENT,
    ARMOR
}

/// <summary>
/// 현재는 ItemInfoLoader를 통해 ScriptableObject인 ItemInfo를 불러오게 하고있음.
/// 기획 정해지고 변경예정
/// </summary>
public static class ItemInfoDataBase
{ 
   public const string ITEMINFO_PATH = "Items";
    
   private static List<ItemInfo> itemInfos;
   public static List<ItemInfo> ItemInfos 
   {
       get
       {
           if (itemInfos == null)
           {
               itemInfos = LoadItemInfoFromDB("");
           }

           return itemInfos;
       }
   }

   public static ItemInfo FindItemInfo(int id)
   {
       return ItemInfos.Find(x => x.id == id);
   }
   
   public static void AddItemInfo(ItemInfo info)
   {
       ItemInfos.Add(info);
   }
   
    //TODO: 기획 정해지고 csv 파싱하기
    public static List<ItemInfo> LoadItemInfoFromDB(string path)
    {
#if TEST_ITEM
        ItemInfo[] infoArr = Resources.LoadAll<ItemInfo>(ITEMINFO_PATH);
        List<ItemInfo> infos = new List<ItemInfo>();
        infos.AddRange(infoArr);
        return infos;
#endif
        return null;
    }
}
