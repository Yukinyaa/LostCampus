using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UIComponent
{
    private static int DEFAULT_SLOT_COUNT = 100;

    private List<UI_ItemSlot> slotList = new List<UI_ItemSlot>();
    private int currentSlotIndex = -1;
    private PlayerInventory inventory; 
    
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup contentLayoutGroup;
    
    public override void Init()
    {
        float width =
            (content.rect.width
             - contentLayoutGroup.padding.left - contentLayoutGroup.padding.right
             - contentLayoutGroup.spacing.x * (contentLayoutGroup.constraintCount - 1))
            / contentLayoutGroup.constraintCount;

        contentLayoutGroup.cellSize = new Vector2(width, width);
        
        slotList.AddRange(content.GetComponentsInChildren<UI_ItemSlot>());
        for(int i=0; i<slotList.Count; i++)
        {
            slotList[i].SetIndex(i);
            if (inventory.Itemslot.Count > i)
            {
                slotList[i].SetSlot(inventory.Itemslot[i]);
            }
        }
    }

    public void SetInventory(PlayerInventory inv)
    {
        inventory = inv;
    }

    public void Toggle()
    {
        if (isActiveAndEnabled)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    public void On()
    {
        gameObject.SetActive(false);
    }
    
    public void Off()
    {
        gameObject.SetActive(true);
    }

    public bool AddSlot(int _slotIndex, Item _slot)
    {
        if (0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            slotList[_slotIndex].SetSlot(_slot);
            return true;
        }
        return false;
    }

    public bool RemoveSlot(int _slotIndex, Item _slot)
    {
        if (0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            slotList[_slotIndex].SetSlot(null);
            return true;
        }
        return false;
    }

    public bool UpdateSlot(int _slotIndex, Item old, Item @new)
    {
        if(0 <= _slotIndex && _slotIndex < slotList.Count)
        {
            slotList[_slotIndex].SetSlot(@new);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        foreach(UI_ItemSlot slot in slotList)
        {
            slot.SetSlot(null);
        }
    }

    private void OnSlotSelected(int _slotIndex)
    {
        currentSlotIndex = _slotIndex;
    }
}
