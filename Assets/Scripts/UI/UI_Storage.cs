using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Storage : UIComponent
{
    private static int DEFAULT_SLOT_COUNT = 100;

    private List<UI_ItemSlot> slotList = new List<UI_ItemSlot>();
    private int currentSlotIndex = -1;
    private ShelterInventory shelterInventory;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup contentLayoutGroup;
    [SerializeField] private CanvasGroup group;

    [Header("- Prefab")]
    [SerializeField] private UI_ItemSlot storageSlot;

    public override void Init()
    {
        float width =
            (viewport.rect.width
            - contentLayoutGroup.padding.left - contentLayoutGroup.padding.right
            - contentLayoutGroup.spacing.x * (contentLayoutGroup.constraintCount - 1))
            / contentLayoutGroup.constraintCount;

       
        contentLayoutGroup.cellSize = new Vector2(width, width);
    }

    private void AddLine()
    {
        for(int i = 0; i < contentLayoutGroup.constraintCount; ++i)
        {
            slotList.Add(Instantiate(storageSlot, content).SetSlot(default));
        }
    }

    public void InitSlot(SyncList<ItemSlot> _slots)
    {
        for(int i = 0; i < _slots.Count; ++i)
        {
            if(i >= slotList.Count) AddLine();
            slotList[i].SetSlot(_slots[i]).SetIndex(i);
        }
    }

    public void AddSlot(int _slotIndex, ItemSlot _slot)
    {
        Debug.Log($"���� �߰� �� : {_slotIndex}");
        if (0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            UI_ItemSlot newSlot = Instantiate(storageSlot, content).SetSlot(_slot).SetIndex(_slotIndex);
            slotList.Insert(_slotIndex, newSlot);
            for(int i = _slotIndex; i < slotList.Count; ++i)
            {
                slotList[i].SetIndex(i);
            }
        }
        else if(slotList.Count <= _slotIndex)
        {
            int newSlotCount = _slotIndex - slotList.Count;
            for(int i = 0; i <= newSlotCount; ++i)
            {
                UI_ItemSlot newSlot = Instantiate(storageSlot, content).SetIndex(slotList.Count);
                if(i == newSlotCount)
                {
                    newSlot.SetSlot(_slot);
                }
                slotList.Add(newSlot);
            }
        }
        if (currentSlotIndex > _slotIndex) currentSlotIndex++;
    }

    public void RemoveSlot(int _slotIndex, ItemSlot _slot)
    {
        Debug.Log($"���� ���� �� : {_slotIndex}");
        if (0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            UI_ItemSlot removeSlot = slotList[_slotIndex];
            slotList.RemoveAt(_slotIndex);
            Destroy(removeSlot.gameObject);
            for(int i = _slotIndex; i < slotList.Count; ++i)
            {
                slotList[i].SetIndex(i);
            }
            if (currentSlotIndex > _slotIndex) currentSlotIndex--;
        }
    }

    public void UpdateSlot(int _slotIndex, ItemSlot _oldSlot, ItemSlot _newSlot)
    {
        Debug.Log($"���� ������Ʈ �� : {_slotIndex}");
        if(0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            UI_ItemSlot currentSlot = slotList[_slotIndex];
            currentSlot.SetSlot(_newSlot);
        }
    }

    public void ClearSlot()
    {
        for(int i = slotList.Count - 1; i >= 0; --i)
        {
            Destroy(slotList[i].gameObject);
        }
        slotList.Clear();
    }

    private void OnClick_Slot(int _slotIndex)
    {
        currentSlotIndex = _slotIndex;
    }
}
