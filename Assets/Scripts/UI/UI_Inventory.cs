using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_Inventory : UIComponent
{
    private List<UI_ItemSlot> slotList = new List<UI_ItemSlot>();
    private int currentSlotIndex = -1;
    private PlayerInventory inventory; 
    
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup contentLayoutGroup;
    [SerializeField] private Transform canvas;
    [SerializeField] private CanvasGroup group;

    private void OnGUI()
    {
#if UNITY_EDITOR
        float width =
            (content.rect.width
             - contentLayoutGroup.padding.left - contentLayoutGroup.padding.right
             - contentLayoutGroup.spacing.x * (contentLayoutGroup.constraintCount - 1))
            / contentLayoutGroup.constraintCount;

        contentLayoutGroup.cellSize = new Vector2(width, width);
#endif
    }

    public void Start()
    {
        canvas = transform.parent;
        float width =
            (content.rect.width
             - contentLayoutGroup.padding.left - contentLayoutGroup.padding.right
             - contentLayoutGroup.spacing.x * (contentLayoutGroup.constraintCount - 1))
            / contentLayoutGroup.constraintCount;

        contentLayoutGroup.cellSize = new Vector2(width, width);
        
        slotList.AddRange(content.GetComponentsInChildren<UI_ItemSlot>());
        for(int i=0; i<slotList.Count; i++)
        {
            slotList[i].SetIndex(i).SetCanvas(canvas);
            if (inventory.Itemslots.Count > i)
            {
                //Debug.Log((object)inventory.Itemslot[i] ==null);
                slotList[i].SetSlot(inventory.Itemslots[i]);
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

    public bool AddSlot(int itemindex, ItemSlot _slot)
    {
        foreach (var uiSlot in slotList)
        {
            if (uiSlot.ItemSlot == null)
            {
                uiSlot.SetSlot(_slot);
                return true;
            }
        }
        return false;
    }

    public bool RemoveSlot(int _slotIndex, ItemSlot _slot)
    {
        foreach (var uiSlot in slotList)
        {
            if (uiSlot.ItemSlot == _slot)
            {
                uiSlot.SetSlot(null);
                return true;
            }
        }
        return false;
    }
    
    public bool ClearSlot()
    {
        foreach (var uiSlot in slotList)
        {
            uiSlot.SetSlot(null);
        }
        return false;
    }

    public bool UpdateSlot(int _slotIndex, ItemSlot old, ItemSlot @new)
    {
        foreach (var uiSlot in slotList)
        {
            if (uiSlot.ItemSlot == old)
            {
                uiSlot.SetSlot(@new);
                return true;
            }
        }
        return false;
    }

    private void OnSlotSelected(int _slotIndex)
    {
        currentSlotIndex = _slotIndex;
    }
    
    
    public void AddItemForTest(int id)
    {
        inventory.TryAddItem(id, 1);
    }
}
