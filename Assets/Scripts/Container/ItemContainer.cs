#define TEST_ITEM

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Mirror;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine.InputSystem;

public class ItemSlotComparer : IComparer<ItemSlot>
{
    public int Compare(ItemSlot x, ItemSlot y)
    {
        throw new NotImplementedException();
    }
}

public abstract class ItemContainer : NetworkBehaviour
{
    [SerializeField]
    protected int MAX_SLOT = 16;
    [SerializeField]
    protected int SLOT_INCREASE_FACTOR = 4;
    
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
    protected void OnStorageUpdatedToServer(SyncList<ItemSlot>.Operation op,int index, ItemSlot oldItem, ItemSlot newItem)
    {
        InvokeSaveInventoryData();
    }
    
    protected void OnStorageUpdated(SyncList<ItemSlot>.Operation op,int index, ItemSlot oldItem, ItemSlot newItem)
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
                    InventoryUI.RemoveSlot(index, oldItem);
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

    protected List<ItemSlot> GetItemDatasByItemID(int _itemID)
    {
        return Itemslots.FindAll(x => x.InfoID == _itemID);
    }
    
    public int GetCountByItemID(int _itemID)
    {
        int count = 0;
        foreach(ItemSlot data in GetItemDatasByItemID(_itemID))
        {
            count += data.Amount;
        }
        return count;
    }

    public int GetEmptySpaceCount(int itemId)
    {
        int maxStack = ItemInfoDataBase.FindItemInfo(itemId).maxStack;
        List<ItemSlot> datas = GetItemDatasByItemID(itemId);
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
    public int TryUpdateItemById(int id, int amount)
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

    [Command(requiresAuthority = false)]
    protected void CmdEditInventoryByItemID(int id, int amount)
    {
        List<ItemSlot> slots = GetItemDatasByItemID(id);
        EditInventoryByItemID(id , amount, slots);
    }
    
    [Server]
    protected void EditInventoryByItemID(int id, int amount, List<ItemSlot> slots)
    {
        ItemInfo info = ItemInfoDataBase.FindItemInfo(id);
        if (slots == null || slots.Count == 0)
        {
            if (amount > 0 && Itemslots.Count < MAX_SLOT)
            {
                int newItemAmount = amount;
                if (amount > info.maxStack)
                {
                    newItemAmount = info.maxStack;
                }
                ItemSlot @new = new ItemSlot(info, newItemAmount);
                Itemslots.Add(@new);
                amount -= newItemAmount;
                EditInventoryByItemID(id, amount, null);
            }
        }
        else
        {
            if (amount < 0) //빼기일때
            {
                ItemSlot currentSlot = slots[0];
                if (currentSlot.Amount > -amount)
                {
                    currentSlot.Amount += amount;
                }
                else if (currentSlot.Amount == amount)
                {
                    _itemslots.Remove(currentSlot);
                }
                else
                {
                    amount += currentSlot.Amount;
                    slots.Remove(currentSlot);
                    _itemslots.Remove(currentSlot);
                    EditInventoryByItemID(id, amount, slots);
                }
            }
            else if(amount > 0)//더하기일때
            {
                ItemSlot currentSlot = slots[0];
                int availableCount = info.maxStack - currentSlot.Amount;
                if (amount > availableCount)
                {
                    amount -= availableCount;
                    currentSlot.Amount = info.maxStack;
                    slots.Remove(currentSlot);
                    EditInventoryByItemID(id, amount, slots);
                }
                else
                {
                    currentSlot.Amount += amount;
                }
            }
        }
    }

    /// <summary>
    /// 빈자리에 채워넣는 것이 아니라 아이템을 [슬롯 하나]로 추가함
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <returns>공간없으면 -1, 있으면 추가한 인덱스</returns>
    public int TryAddItemByID(int id, int amount)
    {
        if (_itemslots.Count >= MAX_SLOT) 
            return -1;
        else
        {
            int idx =  _itemslots.Count;
            CmdAddItemByID(id, amount);
            return _itemslots.Count;
        }
    }
    
    [Command(requiresAuthority = false)]
    protected void CmdAddItemByID(int id, int amount)
    {
        _itemslots.Add(new ItemSlot(ItemInfoDataBase.FindItemInfo(id), amount));
    }
    
    
    /// <summary>
    /// 해당 슬롯 수량 더하기 빼기.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public void TryUpdateItemBySlot(ItemSlot slot, int amount)
    {
        CmdEditInventoryBySlotID(_itemslots.IndexOf(slot), amount);
    }
    
    [Command(requiresAuthority = false)]
    protected void CmdEditInventoryBySlotID(int id, int amount)
    {
        ItemSlot slot = _itemslots[id];
        if (amount < 0) //빼기일때
        {
            if (slot.Amount > -amount)
            {
                slot.Amount += amount;
            }
            else if (slot.Amount <= -amount)
            {
                _itemslots.Remove(slot);
            }
        }
        else if(amount > 0)//더하기일때
        {
            int availableCount = slot.itemInfo.maxStack - slot.Amount;
            if (amount > availableCount)
            {
                slot.Amount = slot.itemInfo.maxStack;
            }
            else
            {
                slot.Amount += amount;
            }
        }
    }
    
    /// <summary>
    /// 해당 슬롯을 바꿔쳐버리기
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="newSlot"></param>
    public void TryChangeItemBySlot(ItemSlot slot, ItemSlot newSlot)
    {
        CmdChangeItemBySlotID(_itemslots.IndexOf(slot), newSlot);
    }
    
    [Command(requiresAuthority = false)]
    protected void CmdChangeItemBySlotID(int id, ItemSlot newSlot)
    {
        _itemslots[id] = newSlot;
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
