using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Storage : UIComponent
{
    private List<UI_StorageSlot> slotList = new List<UI_StorageSlot>();
    private Shelter shelter = null;
    private bool isCashed = false;
    private int currentSlotIndex = -1;

    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup contentLayoutGroup;
    [Header("- Display")]
    [SerializeField] private Image image_Thumbnail;
    [SerializeField] private TextMeshProUGUI text_ID;
    [SerializeField] private TextMeshProUGUI text_Count;
    [Header("- Prefab")]
    [SerializeField] private UI_StorageSlot storageSlot;
    public override void Init()
    {
        float width = viewport.rect.width * .2f;
        contentLayoutGroup.cellSize = new Vector2(width, width);
        contentLayoutGroup.constraintCount = 5;
    }

    public void InitSlot(List<StorageSlot> _slots)
    {
        for(int i = 0; i < _slots.Count; ++i)
        {
            UI_StorageSlot newSlot = Instantiate(storageSlot, content).SetIndex(slotList.Count);
            newSlot.onClick += OnClick_Slot;
            newSlot.SetSlot(_slots[i]);
            slotList.Add(newSlot);
        }
    }

    public void AddSlot(int _slotIndex, StorageSlot _slot)
    {
        Debug.Log($"½½·Ô Ãß°¡ µÊ : {_slotIndex}");
        if (0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            UI_StorageSlot newSlot = Instantiate(storageSlot, content).SetSlot(_slot).SetIndex(_slotIndex);
            newSlot.onClick += OnClick_Slot;
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
                UI_StorageSlot newSlot = Instantiate(storageSlot, content).SetIndex(slotList.Count);
                newSlot.onClick += OnClick_Slot;
                if(i == newSlotCount)
                {
                    newSlot.SetSlot(_slot);
                }
                slotList.Add(newSlot);
            }
        }
        if (currentSlotIndex > _slotIndex) currentSlotIndex++;
    }

    public void RemoveSlot(int _slotIndex, StorageSlot _slot)
    {
        Debug.Log($"½½·Ô Á¦°Å µÊ : {_slotIndex}");
        if (0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            UI_StorageSlot removeSlot = slotList[_slotIndex];
            slotList.RemoveAt(_slotIndex);
            Destroy(removeSlot.gameObject);
            for(int i = _slotIndex; i < slotList.Count; ++i)
            {
                slotList[i].SetIndex(i);
            }
            if (currentSlotIndex > _slotIndex) currentSlotIndex--;
            else if (currentSlotIndex == _slotIndex) DisplayItemData(default(StorageSlot));
        }
    }

    public void UpdateSlot(int _slotIndex, StorageSlot _oldSlot, StorageSlot _newSlot)
    {
        Debug.Log($"½½·Ô ¾÷µ¥ÀÌÆ® µÊ : {_slotIndex}");
        if(0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            UI_StorageSlot currentSlot = slotList[_slotIndex];
            currentSlot.SetSlot(_newSlot);
            if(_slotIndex == currentSlotIndex)
            {
                DisplayItemData(_newSlot);
            }
        }
    }

    public void ClearSlot()
    {
        for(int i = slotList.Count - 1; i >= 0; --i)
        {
            Destroy(slotList[i].gameObject);
        }
        slotList.Clear();
        DisplayItemData(default(StorageSlot));
    }

    private void OnClick_Slot(int _slotIndex)
    {
        currentSlotIndex = _slotIndex;
        if (isCashed)
        {
            DisplayItemData(shelter.storage[_slotIndex]);
        }
    }

    private void DisplayItemData(StorageSlot _slot)
    {
        if (_slot.Equals(default(StorageSlot)))
        {
            text_ID.SetText(string.Empty);
            text_Count.SetText(string.Empty);
        }
        else
        {
            text_ID.SetText(_slot.id.ToString());
            text_Count.SetText(_slot.count.ToString());
        }
    }

    public void OnClick_Debug(int _value)
    {
        if (isCashed)
        {
            if (0 <= currentSlotIndex && currentSlotIndex < slotList.Count)
            {
                shelter.CmdModifyInventoryBySlotID(currentSlotIndex, _value);
            }
        }
    }

    private void Update()
    {
        if(!isCashed)
        {
            shelter = FindObjectOfType<Shelter>();
            if(shelter != null)
            {
                isCashed = true;
            }
        }
    }

}
