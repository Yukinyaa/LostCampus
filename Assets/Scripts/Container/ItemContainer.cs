#define TEST_ITEM

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine.InputSystem;

public abstract class ItemContainer : NetworkBehaviour
{
    [SerializeField]
    protected int MAX_SLOT = 16;
    
   [SerializeField]
   protected SyncList<ItemSlot> _itemslots = new SyncList<ItemSlot>();
    public SyncList<ItemSlot> Itemslots
    {
        get => _itemslots;
    }

    public UI_ItemContainer InventoryUI;

    #region NetworkEvent

    public abstract void cacheUI();
    
    public override void OnStartClient()
    {
        cacheUI();
        _itemslots.Callback += OnStorageUpdated;
        InventoryUI?.SetContainer(this);
    }

    public override void OnStartServer()
    {
        ItemSlot[] data = InvokeLoadInventoryData();
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
    protected void OnStorageUpdatedToServer(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItemSlot, ItemSlot newItemSlot)
    {
        InvokeSaveInventoryData();
    }
    
    protected void OnStorageUpdated(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
    {
        switch (op)
        {
            case SyncList<ItemSlot>.Operation.OP_ADD:
                {
                    InventoryUI.AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_CLEAR:
                {
                    InventoryUI.ClearSlot();
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_INSERT:
                {
                    InventoryUI.AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_REMOVEAT:
                {
                    InventoryUI.RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_SET:
                {
                    InventoryUI.UpdateSlot(index, oldItem, newItem);
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
        return Itemslots.FindAll(x => x.InfoID == _itemID);
    }

    public List<int> GetItemIndexsByItemID(int _itemID)
    {
        List<int> indexs = new List<int>();
        for(int i = 0; i< Itemslots.Count; i++)
        {
            if(Itemslots[i].InfoID == _itemID)
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
    public int TryUpdateItem(int id, int amount)
    {
        if (amount > 0)
        {
            int emptySpaceCount =  GetEmptySpaceCount(id);
            if (emptySpaceCount - amount >= 0)
            {
                CmdEditInventoryByItemID(id, amount);
                return 0;
            }
            else
            {
                CmdEditInventoryByItemID(id, emptySpaceCount);
                return amount - emptySpaceCount;
            }
        }
        else
        {
            return amount;
        }

    }
    
    //TODO:슬롯 업데이트 만들기
    public int TryUpdateSlot(int id, int amount)
    {
        return amount;
    }
    
    [Server]
    protected void CmdEditInventoryBySlotID(int slotIndex, int value)
    {
        if(0 <= slotIndex && slotIndex < Itemslots.Count)
        {
            ItemSlot current = Itemslots[slotIndex];
            if(current.Amount + value > 0)
            {
                if (current.Amount + value <= current.MAXStack)
                {
                    current.Amount = current.Amount + value;
                    Debug.Log(current.Name + " is " + current.Amount);
                }
            }
            else
            {
                Itemslots.RemoveAt(slotIndex);
            }
        }
    }

    [Command(requiresAuthority = false)]
    protected void CmdEditInventoryByItemID(int id, int amount)
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
                else if (!_itemslots[idx].isFull() )
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

    public abstract void InvokeSaveInventoryData();
    public abstract ItemSlot[] InvokeLoadInventoryData();
    
    [Server]
    public void SaveInventoryData(string fileName, string path)
    {
        try
        {
            List<ItemSlot> slots = new List<ItemSlot>();
            _itemslots.CopyTo(slots);
            string json = JsonConvert.SerializeObject(slots);
            SaveLoadHelper.LocalSave(json, fileName, path);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    //TODO: 저장, 로드 만들기2
    [Server]
    public ItemSlot[] LoadInventoryData(string fileName, string path)
    {
#if TEST_ITEM
        ItemSlot[] newItems = new ItemSlot[5];
        for (int i = 0; i < 5; i++)
        {
            newItems[i] = new ItemSlot(ItemInfoDataBase.FindItemInfo(i), i+1);
        }

        return newItems;
#else
        try
        {
            string json;
            SaveLoadHelper.LocalLoad(fileName, path, out json);
            if (json != null)
            {
                List<ItemSlot> slots = JsonConvert.DeserializeObject<List<ItemSlot>>(json);
                Debug.Log("inventory Save Loaded" + json);
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning(e.Message);
        }

#endif
    }

    public void ToggleContainerUI()
    {
        InventoryUI?.Toggle();
    }
    
    public void ActiveContainerUI()
    {
        InventoryUI?.On();
    }
    
}
