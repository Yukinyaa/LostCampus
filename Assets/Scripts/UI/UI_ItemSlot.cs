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

    private Item item;
    private int slotIndex = -1;
    private bool isDragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item.Amount > 0)
        {
            isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
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

    public UI_ItemSlot SetSlot(Item? _slotData)
    {
        if (_slotData == null)
        {
            item = null;
            //image = item.ItemInfo.sprite;
            text.text = "0";
        }
        else
        {
            item = (Item)_slotData;
            //image = item.ItemInfo.sprite;
            text.text = item.Amount.ToString();
        }
        return this;
    }
}
