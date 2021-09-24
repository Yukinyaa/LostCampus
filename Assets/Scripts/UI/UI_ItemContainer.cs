using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_ItemContainer : UIComponent
{
    protected List<UI_ItemSlot> slotList = new List<UI_ItemSlot>();
    protected Dictionary<int, UI_ItemSlot> addExeptionQueue = new Dictionary<int, UI_ItemSlot>();
    protected int currentSlotIndex = -1;
    protected ItemContainer container;
    public ItemContainer Container => container;

    [SerializeField] protected RectTransform content;
    [SerializeField] protected GridLayoutGroup contentLayoutGroup;
    [SerializeField] protected Transform canvas;
    [SerializeField] protected UI_ContainerToolTip tooltip;
    [SerializeField] protected Transform backPanel;
    
    protected Vector2 pivotDiff;
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
        backPanel.GetComponent<UI_EventHandler>().OnDragEvent.AddListener(OnWindowDrag);
        backPanel.GetComponent<UI_EventHandler>().OnBeginDragEvent.AddListener(OnBeginWindowDrag);
        backPanel.GetComponent<UI_EventHandler>().OnDropEvent.AddListener(OnItemDrop);
    }

    public void SetContainer(ItemContainer inv)
    {
        container = inv;
        canvas = transform.parent;
        slotList.AddRange(content.GetComponentsInChildren<UI_ItemSlot>());
        
        for(int i=0; i<slotList.Count; i++)
        {
            InitSlot(i);
        }
        
        for(int i=0; i<container.Itemslots.Count; i++)
        {
            if (slotList.Count > i)
            {
                slotList[i].SetSlot(container.Itemslots[i]);
            }
            else
            {
                AddSlot(i, container.Itemslots[i]);
            }
        }
    }

    public void InitSlot(int idx)
    {
        slotList[idx].SetIndex(idx).SetCanvas(canvas).SetInventoryUI(this);
        slotList[idx].transform.SetParent(content);
    }

    public void ShowToolTip(UI_ItemSlot slot)
    {
        tooltip.setToolTip(slot.ItemSlot, slot.RectTransform.position);
        tooltip.On();
    }
    public void HideToolTip()
    {
        tooltip.Off();
    }

    public virtual bool AddSlot(int itemindex, ItemSlot _slot)
    {
        if (addExeptionQueue.ContainsKey(itemindex))
        {
            addExeptionQueue[itemindex].SetSlot(_slot);
            return true;
        }
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

    public void UpdateItemBySlot(ItemSlot item, UI_ItemSlot slot)
    {
        if (item == null)
        {
            slot.SetSlot(null);
        }
        else
        {
            container.TryChangeItemBySlot(slot.ItemSlot, item);
        }
    }

    private void OnSlotSelected(int _slotIndex)
    {
        currentSlotIndex = _slotIndex;
    }

    protected void OnBeginWindowDrag(UI_EventHandler evHnd, PointerEventData evData )
    {
        pivotDiff = evData.position - (Vector2) evHnd.transform.position;
    }
    
    protected void OnWindowDrag(UI_EventHandler evHnd, PointerEventData evData )
    {
        evHnd.transform.position = evData.position - pivotDiff;
    }
    
    public void OnItemEndDrag(UI_ItemSlot evHnd, PointerEventData evData )
    {
        if (evHnd.ItemSlot == null) return;
        RectTransform rect = backPanel.GetComponent<RectTransform>();
        //Debug.Log($"{evData.position} 인벤크기 {rect.position.x - rect.rect.width/2} ~ {rect.position.x + rect.rect.width/2}, {rect.position.y - rect.rect.height/2} ~ {rect.position.y + rect.rect.height/2}");
        if (evData.position.x > rect.position.x + rect.rect.width/2 
            || evData.position.x < rect.position.x - rect.rect.width/2 
            || evData.position.y > rect.position.y + rect.rect.height/2
            || evData.position.y < rect.position.y - rect.rect.height/2 )
        {
            DropItem(evHnd.ItemSlot);
        }
    }

    private void DropItem(ItemSlot evHndItemSlot)
    {
        throw new NotImplementedException();
        //TODO: 아이템 드롭 구현
    }

    public void AddItemForTest(int amount)
    {
        container.TryUpdateItemById(1, amount);
    }

    public override void Off()
    {
        HideToolTip();
        base.Off();
    }

    public void OnItemDrop(UI_EventHandler evhnd, PointerEventData eventData)
    {
        if (UI_ItemSlot.DraggingSlot != null && UI_ItemSlot.DraggingSlot.ContainerUI != this)
        {
            UI_ItemSlot.DraggingSlot.MoveItemToOtherContainerUI(this);
        }
    }
}
