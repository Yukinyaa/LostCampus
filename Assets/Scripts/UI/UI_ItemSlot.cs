using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform slotContent;
    [SerializeField] private CanvasGroup group;
    
    private ItemSlot _itemSlot;
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

    private UI_Inventory inventoryUI;
    private Transform canvas;
    private bool isDragging = false;

    private static UI_ItemSlot draggingSlot = null;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemSlot != null)
        {
            isDragging = true;
            slotContent.SetParent(canvas);
            group.blocksRaycasts = false;
            group.interactable = false;
            draggingSlot = this;
            Debug.Log("drag: " + draggingSlot.ItemSlot.Amount + "data : " + _itemSlot.Amount);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            slotContent.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggingSlot != null)
        {
                ItemSlot tempItemSlot = ItemSlot;
                SetSlot(draggingSlot.ItemSlot);
                draggingSlot.SetSlot(tempItemSlot);
                draggingSlot = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slotContent.SetParent(transform);
        slotContent.localPosition = Vector3.zero;
        group.blocksRaycasts = true;
        group.interactable = true;
        isDragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                break;
            case PointerEventData.InputButton.Right:
                break;
        }
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
            if (_itemSlot != null) _itemSlot.OnValueChanged -= OnValueChanged;
            _itemSlot = null;
            image.sprite = ItemInfoDataBase.GetSprite("None");
            text.text = "0";
        }
        else
        {
            _itemSlot = (ItemSlot)_slotData;
            image.sprite = ItemInfoDataBase.GetSprite(_slotData.Sprite);
            _itemSlot.OnValueChanged += OnValueChanged;
            text.text = _itemSlot.Amount.ToString();
        }
        return this;
    }

}
