using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShelterInventory : NetworkBehaviour
{
    [SerializeField]
    private SyncList<ItemSlot> _itemslots = new SyncList<ItemSlot>();
    public SyncList<ItemSlot> Itemslots
    {
        get => _itemslots;
    }

    public override void OnStartClient()
    {
        _itemslots.Callback += OnStorageUpdated;
        UIManager.Instance.GetUI<UI_Storage>().InitSlot(Itemslots);
    }

    public override void OnStartServer()
    {
        ItemSlot[] data = LoadInventoryData();
        for (int i = 0; i < data.Length; ++i)
        {
            _itemslots.Add(data[i]);
        }
        _itemslots.Callback += OnStorageUpdatedToServer;
    }

    private void OnStorageUpdated(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
    {
        switch (op)
        {
            case SyncList<ItemSlot>.Operation.OP_ADD:
                {
                    // 새로운 아이템 슬롯이 추가되었을 때
                    // index = 추가된 아이템 슬롯의 index
                    // newItem = 추가된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_CLEAR:
                {
                    // 리스트가 초기화 되었을 때
                    UIManager.Instance.GetUI<UI_Storage>().ClearSlot();
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_INSERT:
                {
                    // 리스트 중간에 삽입되었을 때
                    // index = 추가된 아이템 슬롯의 index
                    // newItem = 추가된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_REMOVEAT:
                {
                    // 아이템 슬롯이 제거되었을 때
                    // index = 제거된 아이템 슬롯의 index
                    // oldItem = 제거된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_SET:
                {
                    // 기존 아이템 슬롯이 업데이트 되었을 때 (수치의 증감, 슬롯 교환 등)
                    // index = 업데이트 된 아이템 슬롯의 index
                    // oldItem = 업데이트 되기 이전의 아이템 슬롯
                    // newItem = 업데이트 된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().UpdateSlot(index, oldItem, newItem);
                    break;
                }
        }
    }
    private void OnStorageUpdatedToServer(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
    {
        SaveInventoryData();
    }

    [Server]
    public void SaveInventoryData()
    {
        try
        {
            List<ItemSlot> storageData = new List<ItemSlot>(_itemslots.Count);
            _itemslots.CopyTo(storageData);
            string json = JsonHelper.ToJson(storageData.ToArray(), true);
            string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {

        }
    }

    [Server]
    public ItemSlot[] LoadInventoryData()
    {
        string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonHelper.FromJson<ItemSlot>(json);
            }
            else
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Server/");
                ItemSlot[] newData = new ItemSlot[5];
                for (int i = 0; i < 5; ++i)
                {
                    newData[i] = new ItemSlot(ItemInfoDataBase.FindItemInfo(i), i);
                }
                File.WriteAllText(path, JsonHelper.ToJson(newData, true));
                return newData;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }


}
