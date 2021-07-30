using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_ItemContainer : UIComponent
{
    protected List<UI_ItemSlot> slotList = new List<UI_ItemSlot>();
    protected int currentSlotIndex = -1;
    protected ItemContainer container; 
    
    [SerializeField] protected RectTransform content;
    [SerializeField] protected GridLayoutGroup contentLayoutGroup;
    [SerializeField] protected Transform canvas;
    protected void OnGUI()
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
        float width =
            (content.rect.width
             - contentLayoutGroup.padding.left - contentLayoutGroup.padding.right
             - contentLayoutGroup.spacing.x * (contentLayoutGroup.constraintCount - 1))
            / contentLayoutGroup.constraintCount;

        contentLayoutGroup.cellSize = new Vector2(width, width);
    }

    public void SetContainer(ItemContainer inv)
    {
        container = inv;
        canvas = transform.parent;
        slotList.AddRange(content.GetComponentsInChildren<UI_ItemSlot>());
        for(int i=0; i<slotList.Count; i++)
        {
            slotList[i].SetIndex(i).SetCanvas(canvas);
            if (container.Itemslots.Count > i)
            {
                //Debug.Log((object)inventory.Itemslot[i] ==null);
                slotList[i].SetSlot(container.Itemslots[i]);
            }
        }
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

    public virtual bool AddSlot(int itemindex, ItemSlot _slot)
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

    public virtual bool RemoveSlot(int _slotIndex, ItemSlot _slot)
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
    
    public virtual void ClearSlot()
    {
        foreach (var uiSlot in slotList)
        {
            uiSlot.SetSlot(null);
        }
    }

    public virtual bool UpdateSlot(int _slotIndex, ItemSlot old, ItemSlot @new)
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
        container.TryUpdateItem(id, 1);
    }
}
