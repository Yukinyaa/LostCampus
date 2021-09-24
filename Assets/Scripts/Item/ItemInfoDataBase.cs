#define TEST_ITEM

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: enum 정리하기
[Serializable]
[Flags]
public enum ItemType
{
    None = 0,
    // ---------------------------- Weapon ----------------------------
    Dagger = 1 << 0,                                        // 단검
    StraightSword = 1 << 1,                                 // 직검
    CurvedSword = 1 << 2,                                   // 곡검
    GreatSword = 1 << 3,                                    // 대검
    Hammer = 1 << 4,                                        // 둔기
    GreatHammer = 1 << 5,                                   // 대형 둔기
    Spear = 1 << 6,                                         // 창
    Bow = 1 << 7,                                           // 활
    Crossbow = 1 << 8,                                      // 쇠뇌
    Gun = 1 << 9,                                           // 총
    Shield = 1 << 10,                                       // 방패
    // ---------------------------- Ammo ----------------------------
    Arrow = 1 << 11,                                        // 화살
    Bolt = 1 << 12,                                         // 볼트
    Bullet = 1 << 13,                                       // 총알
    // ---------------------------- Armor ----------------------------
    HeadGear = 1 << 14,                                     // 투구
    ChestGear = 1 << 15,                                    // 갑옷
    HandGear = 1 << 16,                                     // 장갑
    WaistGear = 1 << 17,                                    // 허리띠
    ThighGear = 1 << 18,                                    // 레깅스
    FootGear = 1 << 19,                                     // 장화
    // ---------------------------- Stuff ----------------------------
    Harvest = 1 << 20,                                      // 채집해서 얻는 재료
    Hunt = 1 << 21,                                         // 수렵해서 얻는 재료
    Craft = 1 << 22,                                        // 조합해서 얻는 재료
    Supply = 1 << 23,                                       // 소모품
    // ---------------------------- Special ----------------------------
    Special = 1 << 24,                                      // 중요 아이템
    // ---------------------------- Category ----------------------------
    Weapon = Dagger | StraightSword | CurvedSword | GreatSword | Hammer | GreatHammer | Spear | Bow | Crossbow | Gun | Shield,
    Ammo = Arrow | Bolt | Bullet,
    Armor = HeadGear | ChestGear | HandGear | WaistGear | ThighGear | FootGear,
    Stuff = Harvest | Hunt | Craft | Supply
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
