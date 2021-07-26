using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
public class UI_StorageSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    public int slotIndex = -1;
    public ItemSlot slotData;

    private bool isDragging = false;
    [SerializeField] private ItemDataContainer itemContainer;
    [SerializeField] private ItemDataContainer dragAndDropContainer;
    
    /*
    public void OnBeginDrag(PointerEventData eventData)
    {
        return; // 일단 버그나서 고쳐야댐.
        Debug.Log(name + " 에서 드래그 시작");
        if (slotData.Amount > 0)
        {
            isDragging = true;
            ItemDataContainer.container = dragAndDropContainer.SetData(slotData).SetActive(true).SetParent(UIManager.Instance.transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            dragAndDropContainer.transform.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(ItemDataContainer.container != null)
        {
            Debug.Log(name + " 에 드롭");
            StorageSlot itemData = ItemDataContainer.container.itemData;
            if(itemData.id == slotData.id)
            {
                slotData.count += itemData.count;
                ItemDataContainer.container = null;
            }
            else
            {
                ItemDataContainer.container.itemData = slotData;
                SetSlot(itemData);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(name + " 에서 드래그 끝");
        if(ItemDataContainer.container != null)
        {
            SetSlot(ItemDataContainer.container.itemData);
            ItemDataContainer.container = null;
        }
        dragAndDropContainer.transform.localPosition = Vector3.zero;
        dragAndDropContainer.SetActive(false).SetParent(transform);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                {
                    Debug.Log("좌클릭");
                    break;
                }
            case PointerEventData.InputButton.Right:
                {
                    Debug.Log("우클릭");
                    break;
                }
        }
    }

    public UI_StorageSlot SetIndex(int _index)
    {
        slotIndex = _index;
        transform.SetSiblingIndex(_index);
        return this;
    }

    public UI_StorageSlot SetSlot(ItemSlot _slotData)
    {
        slotData = _slotData;
        itemContainer.SetData(_slotData);
        return this;
    }

    private void Update()
    {
        if (isDragging)
        {
            dragAndDropContainer.transform.position = Mouse.current.position.ReadValue();
        }
    }
}
*/