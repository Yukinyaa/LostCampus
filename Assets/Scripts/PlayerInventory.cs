using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInventory : NetworkBehaviour
{
    private SyncList<Item> _itemslot = new SyncList<Item>();
    public SyncList<Item> Itemslot
    {
        get => _itemslot;
    }

    public UI_Inventory InventoryUI;

    public const int MAX_SLOT = 16;

    private StarterAssetsInputs input;


    public override void OnStartClient()
    {
        input = transform.GetComponent<StarterAssetsInputs>();
        if (isLocalPlayer)
        {
            InventoryUI = UIManager.Instance.GetUI<UI_Inventory>();
            InventoryUI.SetInventory(this);
        }
        _itemslot.Callback += OnStorageUpdated;
    }

    public override void OnStartServer()
    {
        Item[] data = LoadInventoryData();
        if (data != null)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                Itemslot.Add(data[i]);
            }
        }

        _itemslot.Callback += OnStorageUpdatedToServer;
    }
    private void OnStorageUpdated(SyncList<Item>.Operation op, int index, Item oldItem, Item newItem)
    {
        switch (op)
        {
            case SyncList<Item>.Operation.OP_ADD:
                {
                    InventoryUI?.AddSlot(index, newItem);
                    break;
                }
            case SyncList<Item>.Operation.OP_CLEAR:
                {
                    InventoryUI?.Clear();
                    break;
                }
            case SyncList<Item>.Operation.OP_INSERT:
            {
                    InventoryUI?.AddSlot(index, newItem);
                    break;
                }
            case SyncList<Item>.Operation.OP_REMOVEAT:
                {
                    InventoryUI?.RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<Item>.Operation.OP_SET:
                {
                    InventoryUI?.UpdateSlot(index, oldItem, newItem);
                    break;
                }
        }
    }

    [Server]
    private void OnStorageUpdatedToServer(SyncList<Item>.Operation op, int index, Item oldItem, Item newItem)
    {
        SaveInventoryData();
    }


    public Item GetItemDataBySlotID(int _slotID)
    {
        if (0 <= _slotID && _slotID < Itemslot.Count)
        {
            return Itemslot[_slotID];
        }
        return default;
    }

    public List<Item> GetItemDataByItemID(int _itemID)
    {
        return Itemslot.FindAll(x => x.ItemInfo.id == _itemID);
    }

    public int GetCountByItemID(int _itemID)
    {
        int count = 0;
        foreach(Item data in GetItemDataByItemID(_itemID))
        {
            count += data.Amount;
        }
        return count;
    }

    [Command]
    public bool CmdEditInventoryBySlotID(int slotIndex, int value)
    {
        if(0 <= slotIndex && slotIndex < Itemslot.Count)
        {
            Item current = Itemslot[slotIndex];
            if(current.Amount + value > 0)
            {
                current.Amount = current.Amount + value;
                return true;
            }
            else
            {
                Itemslot.RemoveAt(slotIndex);
                return true;
            }
        }
        return false;
    }

    [Command]
    public bool CmdEditInventoryByItemID(int id, int value)
    {
        int slotIndex = Itemslot.FindIndex(x => x.ItemInfo.id == id);
        if (slotIndex == -1)
        {
            if (value > 0 && Itemslot.Count < MAX_SLOT)
            {
                Item @new = new Item(ItemInfoDataBase.FindItemInfo(id), value);
                Itemslot.Add(@new);
                return true;
            }
            return false;
        }
        return CmdEditInventoryBySlotID(slotIndex, value);
    }

    //TODO: 저장, 로드 만들기
    [Server]
    public void SaveInventoryData()
    {
        // try
        // {
        //     string json = JsonHelper.ToJson(storageData.ToArray(), true);
        //     string path = Application.persistentDataPath + "/Server/" + NetworkIdentity. +"_Inventory.txt";
        //     File.WriteAllText(path, json);
        // }
        // catch (Exception)
        // {
        // }
    }

    [Server]
    public Item[] LoadInventoryData()
    {
        return null;
        // string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
        // try
        // {
        //     if (File.Exists(path))
        //     {
        //         string json = File.ReadAllText(path);
        //         //return JsonHelper.FromJson<StorageSlot>(json);
        //     }
        //     else
        //     {
        //         Directory.CreateDirectory(Application.persistentDataPath + "/Server/");
        //         StorageSlot[] newData = new StorageSlot[5];
        //         for (int i = 0; i < 5; ++i)
        //         {
        //             newData[i] = new StorageSlot(i, 100);
        //         }
        //         File.WriteAllText(path, JsonHelper.ToJson(newData, true));
        //         return newData;
        //     }
        // }
        // catch (Exception)
        // {
        //     return null;
        // }
    }

    public void OnToggleInventory(InputValue value)
    {
        InventoryUI?.Toggle();
    }
    
}
