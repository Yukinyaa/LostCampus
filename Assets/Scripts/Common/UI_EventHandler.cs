using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
    , IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent<UI_EventHandler, PointerEventData> OnBeginDragEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    public UnityEvent<UI_EventHandler, PointerEventData> OnDragEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    public UnityEvent<UI_EventHandler, PointerEventData> OnDropEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    public UnityEvent<UI_EventHandler, PointerEventData> OnEndDragEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    public UnityEvent<UI_EventHandler, PointerEventData> OnPointerEnterEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    public UnityEvent<UI_EventHandler, PointerEventData> OnPointerExitEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    public UnityEvent<UI_EventHandler, PointerEventData> OnPointerClickEvent = new UnityEvent<UI_EventHandler, PointerEventData>();
    
    private bool isDragging = false;
    private bool isPointerFloating = false;
    public bool IsDragging => isDragging;
    public bool IsPointerFloating => isPointerFloating;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        OnBeginDragEvent.Invoke(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent.Invoke(this, eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnDropEvent.Invoke(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        OnEndDragEvent.Invoke(this, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerFloating = true;
        OnPointerEnterEvent.Invoke(this, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerFloating = false;
        OnPointerExitEvent.Invoke(this, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickEvent.Invoke(this, eventData);
    }

    
}
