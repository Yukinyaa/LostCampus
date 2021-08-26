using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MyItemData
{
    public int id;
    public string name;
    public string description;
    public ItemCategory category;
}

public class MyItemManager : MonoBehaviour
{
    public static MyItemManager Instance { get; private set; } = null;

    private Dictionary<int, MyItemData> itemData;
    private Dictionary<int, Sprite> spriteData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MyItemData[] data = JsonHelper.FromJson<MyItemData>(Resources.Load<TextAsset>("Item").text);
        itemData = new Dictionary<int, MyItemData>(data.Length);
        for(int i = 0; i < data.Length; ++i)
        {
            itemData.Add(data[i].id, data[i]);
        }

        Sprite[] sData = Resources.LoadAll<Sprite>("ItemImage");
        spriteData = new Dictionary<int, Sprite>(sData.Length);
        for(int i = 0; i < sData.Length; ++i)
        {
            if(int.TryParse(sData[i].name, out int index))
            {
                spriteData.Add(index, sData[i]);
            }
            else
            {
                if (sData[i].name.Equals("None"))
                {
                    spriteData.Add(-1, sData[i]);
                }
            }
        }
    }

    public MyItemData GetItemData(int _itemID)
    {
        if (itemData.ContainsKey(_itemID))
        {
            return itemData[_itemID];
        }
        return default;
    }

    public Sprite GetSprite(int _itemID)
    {
        if (spriteData.ContainsKey(_itemID))
        {
            return spriteData[_itemID];
        }
        return spriteData[-1];
    }

}
