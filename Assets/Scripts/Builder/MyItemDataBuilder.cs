using System;
using System.IO;
using UnityEngine;

public class MyItemDataBuilder : MonoBehaviour
{
    public MyItemData[] itemData;

    private void Awake()
    {
        Destroy(gameObject);
    }

    public void BuildData()
    {
        SaveItemData();
    }

    public void SaveItemData()
    {
        try
        {
            string json = JsonHelper.ToJson(itemData, true);
            string path = Application.dataPath + "/Resources/" + "Item.txt";
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {

        }
    }
}
