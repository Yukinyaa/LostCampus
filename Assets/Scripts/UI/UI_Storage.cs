using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Storage : UI_ItemContainer
{
    private static int DEFAULT_SLOT_COUNT = 100;

    [Header("- Prefab")]
    [SerializeField] private UI_ItemSlot storageSlot;


    private void AddLine()
    {
        for(int i = 0; i < contentLayoutGroup.constraintCount; ++i)
        {
            slotList.Add(Instantiate(storageSlot, content).SetSlot(default));
        }
    }

    public override bool AddSlot(int _slotIndex, ItemSlot _slot)
    {
        Debug.Log($"½½·Ô Ãß°¡ µÊ");
        UI_ItemSlot newSlot = Instantiate(storageSlot, content);
        slotList.Add(newSlot);
        InitSlot(slotList.IndexOf(newSlot));
        newSlot.SetSlot(_slot);
        return true;
    }

    public override bool RemoveSlot(int _slotIndex, ItemSlot _slot)
    {
        foreach (var uiSlot in slotList)
        {
            if (uiSlot.ItemSlot == _slot)
            {
                slotList.Remove(uiSlot);
                Destroy(uiSlot.SlotContent.gameObject);
                Destroy(uiSlot.gameObject);
                return true;
            }
        }
        return false;
    }

    public override void ClearSlot()
    {
        for(int i = slotList.Count - 1; i >= 0; --i)
        {
            Destroy(slotList[i].gameObject);
        }
        slotList.Clear();
    }
}
