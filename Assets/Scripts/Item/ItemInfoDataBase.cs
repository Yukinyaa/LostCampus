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
    None = 0,
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
   public const string ITEMIMAGE_PATH = "ItemImage";

   private static List<ItemInfo> itemInfos;
   public static List<ItemInfo> ItemInfos 
   {
       get
       {
           if (itemInfos == null)
           {
               itemInfos = LoadItemInfoFromDB(ITEMINFO_PATH);
           }

           return itemInfos;
       }
   }
   
   private static Dictionary<string, Sprite> sprites;
   public static Dictionary<string, Sprite> Sprites 
   {
       get
       {
           if (sprites == null)
           {
               sprites = LoadItemSpriteFromDB(ITEMIMAGE_PATH);
           }

           return sprites;
       }
   }

   public static ItemInfo FindItemInfo(int id)
   {
       return ItemInfos.Find(x => x.id == id);
   }

   public static Sprite GetSprite(string name)
   {
       Sprite spr = Sprites[name];
       if (spr == null) return Sprites["None"];
       return Sprites[name];
   }
   
   public static void AddItemInfo(ItemInfo info)
   {
       ItemInfos.Add(info);
   }
   
    //TODO: 기획 정해지고 csv 파싱하기
    public static List<ItemInfo> LoadItemInfoFromDB(string path)
    {
#if TEST_ITEM
        ItemInfo[] infoArr = Resources.LoadAll<ItemInfo>(path);
        List<ItemInfo> infos = new List<ItemInfo>();
        infos.AddRange(infoArr);
        return infos;
#endif
        return null;
    }
    
    public static Dictionary<string, Sprite> LoadItemSpriteFromDB(string path)
    {
#if TEST_ITEM
        Sprite[] sData = Resources.LoadAll<Sprite>(path);
        Dictionary<string, Sprite> spriteData = new Dictionary<string, Sprite>();
        for(int i = 0; i < sData.Length; i++)
        {
            spriteData.Add(sData[i].name, sData[i]);
        }
        return spriteData;
#endif
        return null;
    }
}
