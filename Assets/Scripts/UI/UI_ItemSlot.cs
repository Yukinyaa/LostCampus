using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
    , IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected CanvasGroup group;
    [SerializeField] protected RectTransform slotContent;
    public RectTransform SlotContent => slotContent;

    
    protected RectTransform rectTransform;
    protected UnityEvent<UI_ItemSlot, PointerEventData> onEndDragEvent =new UnityEvent<UI_ItemSlot, PointerEventData>();
    public UnityEvent<UI_ItemSlot, PointerEventData> OnEndDragEvent
    {
        get => onEndDragEvent;
        set => onEndDragEvent = value;
    }

    public RectTransform RectTransform
    {
        get
        {
            if ((object)rectTransform == null)
            {
                rectTransform = transform.GetComponent<RectTransform>();
            }

            return rectTransform;
        }
    }
    
    protected ItemSlot _itemSlot;
    public ItemSlot ItemSlot
    {
        get => _itemSlot;
        set => _itemSlot = value;
    }
    private int slotIndex = -1;
    public int SlotIndex
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    private UI_ItemContainer containerUI;
    public UI_ItemContainer ContainerUI
    {
        get => containerUI;
    }

    private Transform canvas;
    private static bool isDragging = false;

    private static UI_ItemSlot draggingSlot = null;
    public static UI_ItemSlot DraggingSlot
    {
        get => draggingSlot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemSlot != null)
        {
            containerUI.HideToolTip();
            isDragging = true;
            slotContent.SetParent(canvas);
            group.blocksRaycasts = false;
            group.interactable = false;
            draggingSlot = this;
//            Debug.Log("drag: " + draggingSlot.ItemSlot.Amount + "data : " + _itemSlot.Amount);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && (object)_itemSlot != null)
        {
            slotContent.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggingSlot != null)
        {
            if (IsInSameContainerUI(draggingSlot))
            {
                ItemSlot tempItemSlot = ItemSlot;
                SetSlot(draggingSlot.ItemSlot);
                draggingSlot.SetSlot(tempItemSlot);
                draggingSlot = null;
            }
            // else if ((object) _itemSlot == null)
            // {
            //     containerUI.UpdateItemBySlot(draggingSlot._itemSlot, this);
            //     draggingSlot.containerUI.UpdateItemBySlot(null, draggingSlot);
            //     draggingSlot.containerUI.Container.TryUpdateItemBySlot(draggingSlot._itemSlot, -draggingSlot._itemSlot.Amount);
            // }
            // else
            // {
            //     if (_itemSlot.InfoID == draggingSlot._itemSlot.InfoID)
            //     {
            //         int remain = containerUI.Container.TryUpdateItemBySlot(_itemSlot, draggingSlot._itemSlot.Amount);
            //         draggingSlot.containerUI.Container.TryUpdateItemBySlot(draggingSlot._itemSlot, -(draggingSlot._itemSlot.Amount - remain));
            //     }
            //     else
            //     {
            //         ItemSlot tempslot = _itemSlot;
            //         containerUI.UpdateItemBySlot(draggingSlot._itemSlot, this);
            //         draggingSlot.containerUI.UpdateItemBySlot(tempslot, draggingSlot);
            //     }
            //    }
        }
        

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotContent.SetParent(transform);
        slotContent.localPosition = Vector3.zero;
        group.blocksRaycasts = true;
        group.interactable = true;
        isDragging = false;
        onEndDragEvent.Invoke(this, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemSlot == null || isDragging) return;
        containerUI.ShowToolTip(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_itemSlot == null) return;
        containerUI.HideToolTip();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                break;
            case PointerEventData.InputButton.Right:
            {
                if (_itemSlot == null) return;
                if (IsRivalContainerUIActive())
                {
                    MoveItemToRivalContainerUI();
                }

                else if (_itemSlot is UsableItem)
                {
                    var temp = _itemSlot as UsableItem;
                    if (temp.Use())
                        containerUI.Container.TryUpdateItemBySlot(_itemSlot, -1);
                }

                else if (_itemSlot is EquipableItem)
                {
                    var temp = _itemSlot as EquipableItem;
                    if (temp.Equip())
                        containerUI.Container.TryUpdateItemBySlot(_itemSlot, -1);
                }

                break;
            }
        }
    }

    public void MoveItemToOtherContainerUI(UI_ItemContainer container)
    {
        int remain = AddItemToOtherContainerUI(container);
        if (remain > 0)
        {
            _itemSlot.Amount = remain;
        }
        else if (remain == 0)
        {
            containerUI.Container.TryUpdateItemBySlot(_itemSlot, -_itemSlot.Amount);
            containerUI.HideToolTip();
        }
    }
    
    public int AddItemToOtherContainerUI(UI_ItemContainer containerUI)
    {
        int remain = containerUI.Container.TryUpdateItemById(_itemSlot.InfoID, _itemSlot.Amount);
        return remain;
    }
    
    public virtual UI_ItemContainer GetRivalContainerUI()
    {
        return null;
    }
    
    public void MoveItemToRivalContainerUI()
    {
        MoveItemToOtherContainerUI(GetRivalContainerUI());
    }
    
    public bool IsRivalContainerUIActive()
    {
        if (GetRivalContainerUI() == null) return false;
        return GetRivalContainerUI().isActiveAndEnabled;
    }
    
    public virtual bool IsInSameContainerUI(UI_ItemSlot slot)
    {
        return containerUI == slot.containerUI;
    }

    public UI_ItemSlot SetIndex(int _index)
    {
        slotIndex = _index;
        transform.SetSiblingIndex(_index);
        return this;
    }

    public UI_ItemSlot SetCanvas(Transform _canvas)
    {
        canvas = _canvas;
        return this;
    }
    
    public UI_ItemSlot SetInventoryUI(UI_ItemContainer inv)
    {
        containerUI = inv;
        onEndDragEvent.AddListener(inv.OnItemEndDrag);
        return this;
    }
    
    public UI_ItemSlot SetCanvasGroup(CanvasGroup grp)
    {
        group = grp;
        return this;
    }

    public void OnValueChanged(ItemSlot item)
    {
        if (item.Amount <= 0) SetSlot(null);
        else text.text = _itemSlot.Amount.ToString();
    }
    
    public UI_ItemSlot SetSlot(ItemSlot _slotData)
    {
        if (_slotData == null)
        {
            if (_itemSlot != null) 
                _itemSlot.OnValueChanged -= OnValueChanged;
            _itemSlot = null;
            image.sprite = ItemInfoDataBase.GetSprite("None");
            text.text = "0";
        }
        else
        {
            if(_itemSlot != null) 
                _itemSlot.OnValueChanged -= OnValueChanged;
            
            _itemSlot = (ItemSlot)_slotData;
            image.sprite = ItemInfoDataBase.GetSprite(_slotData.Sprite);
            _itemSlot.OnValueChanged += OnValueChanged;
            text.text = _itemSlot.Amount.ToString();
        }
        return this;
    }
}
