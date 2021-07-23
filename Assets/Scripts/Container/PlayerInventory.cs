using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField]
    private SyncList<ItemSlot> _itemslots = new SyncList<ItemSlot>();
    public SyncList<ItemSlot> Itemslots
    {
        get => _itemslots;
    }

    public UI_Inventory InventoryUI;

    public const int MAX_SLOT = 16;

    private StarterAssetsInputs input;

    #region NetworkEvent

    public override void OnStartLocalPlayer()
    {
        input = transform.GetComponent<StarterAssetsInputs>();
        if (isLocalPlayer)
        {
            InventoryUI = UIManager.Instance.GetUI<UI_Inventory>();
            InventoryUI.SetInventory(this);
        }
    }

    public override void OnStartServer()
    {
        ItemSlot[] data = LoadInventoryData();
        if (data != null)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                Itemslots.Add(data[i]);
            }
        }
        _itemslots.Callback += OnStorageUpdatedToServer;
    }

    [Server]
    private void OnStorageUpdatedToServer(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItemSlot, ItemSlot newItemSlot)
    {
        SaveInventoryData();
    }
    
    private void OnStorageUpdated(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
    {
        switch (op)
        {
            case SyncList<ItemSlot>.Operation.OP_ADD:
                {
                    UIManager.Instance.GetUI<UI_Inventory>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_CLEAR:
                {
                    UIManager.Instance.GetUI<UI_Inventory>().ClearSlot();
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_INSERT:
                {
                    UIManager.Instance.GetUI<UI_Inventory>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_REMOVEAT:
                {
                    UIManager.Instance.GetUI<UI_Inventory>().RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_SET:
                {
                    UIManager.Instance.GetUI<UI_Inventory>().UpdateSlot(index, oldItem, newItem);
                    break;
                }
        }
    }
    #endregion

    #region Modify

    public ItemSlot GetItemDataBySlotID(int _slotID)
    {
        if (0 <= _slotID && _slotID < Itemslots.Count)
        {
            return Itemslots[_slotID];
        }
        return default;
    }

    public List<ItemSlot> GetItemDataByItemID(int _itemID)
    {
        return Itemslots.FindAll(x => x.ItemInfo.id == _itemID);
    }

    public List<int> GetItemIndexsByItemID(int _itemID)
    {
        List<int> indexs = new List<int>();
        for(int i = 0; i< Itemslots.Count; i++)
        {
            if(Itemslots[i].ItemInfo.id == _itemID)
                indexs.Add(i);
        }
        return indexs;
    }
    
    public int GetCountByItemID(int _itemID)
    {
        int count = 0;
        foreach(ItemSlot data in GetItemDataByItemID(_itemID))
        {
            count += data.Amount;
        }
        return count;
    }

    public int GetEmptySpaceCount(int itemId)
    {
        int maxStack = ItemInfoDataBase.FindItemInfo(itemId).maxStack;
        List<ItemSlot> datas = GetItemDataByItemID(itemId);
        int count = 0;
        foreach (var VARIABLE in datas)
        {
            count += maxStack - VARIABLE.Amount;
        }
        count += (MAX_SLOT - Itemslots.Count) * maxStack;
        
        return count;
    }

    /// <summary>
    /// 인벤에 아이템을 넣도록 시도함
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <returns>인벤에 넣고 남은 갯수</returns>
    public int TryAddItem(int id, int amount)
    {
        int emptySpaceCount =  GetEmptySpaceCount(id);
        if (GetEmptySpaceCount(id) - amount >= 0)
        {
            CmdEditInventoryByItemID(id, amount);
            return 0;
        }
        
        return amount - emptySpaceCount;
    }
    
    [Server]
    private void CmdEditInventoryBySlotID(int slotIndex, int value)
    {
        if(0 <= slotIndex && slotIndex < Itemslots.Count)
        {
            ItemSlot current = Itemslots[slotIndex];
            if(current.Amount + value > 0)
            {
                if (current.Amount + value <= current.ItemInfo.maxStack)
                {
                    current.Amount = current.Amount + value;
                    Debug.Log(current.ItemInfo.id + " is " + current.Amount);
                }
            }
            
            else
            {
                Itemslots.RemoveAt(slotIndex);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdEditInventoryByItemID(int id, int amount)
    {
        List<int> indexes = GetItemIndexsByItemID(id);
        ItemInfo info = ItemInfoDataBase.FindItemInfo(id);
        if (indexes.Count == 0)
        {
            if (amount > 0 && Itemslots.Count < MAX_SLOT)
            {
                for (int i = amount; i > 0;)
                {
                    ItemSlot @new = new ItemSlot(info, amount);
                    Itemslots.Add(@new);
                    i = i - info.maxStack;
                }
            }
        }
        else
        {
            foreach (var idx in indexes)
            {
                if (amount <= 0) break;
                if (!_itemslots[idx].isFull() )
                {
                    int availableCount = info.maxStack - _itemslots[idx].Amount;
                    if (amount > availableCount)
                    {
                        amount -= availableCount;
                        CmdEditInventoryBySlotID(idx, availableCount);
                    }
                    else
                    {
                        CmdEditInventoryBySlotID(idx, amount);
                    }
                }
            }
        }
    }

    #endregion

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
    public ItemSlot[] LoadInventoryData()
    {
        ItemSlot[] newItems = new ItemSlot[5];
        //return null;
        for (int i = 0; i < 5; i++)
        {
            newItems[i] = new ItemSlot(ItemInfoDataBase.FindItemInfo(i), i);
        }

        return newItems;
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
        if(value.isPressed)
            InventoryUI?.Toggle();
    }
    
}
