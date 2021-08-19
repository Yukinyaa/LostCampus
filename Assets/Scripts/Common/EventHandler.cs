using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
    , IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private UnityEvent<EventHandler, PointerEventData> onBeginDragEvent = new UnityEvent<EventHandler, PointerEventData>();
    private UnityEvent<EventHandler, PointerEventData> onDragEvent = new UnityEvent<EventHandler, PointerEventData>();
    private UnityEvent<EventHandler, PointerEventData> onDropEvent = new UnityEvent<EventHandler, PointerEventData>();
    private UnityEvent<EventHandler, PointerEventData> onEndDragEvent = new UnityEvent<EventHandler, PointerEventData>();
    private UnityEvent<EventHandler, PointerEventData> onPointerEnterEvent = new UnityEvent<EventHandler, PointerEventData>();
    private UnityEvent<EventHandler, PointerEventData> onPointerExitEvent = new UnityEvent<EventHandler, PointerEventData>();
    private UnityEvent<EventHandler, PointerEventData> onPointerClickEvent = new UnityEvent<EventHandler, PointerEventData>();

    public UnityEvent<EventHandler, PointerEventData> OnBeginDragEvent
    {
        get => onBeginDragEvent;
        set => onBeginDragEvent = value;
    }

    public UnityEvent<EventHandler, PointerEventData> OnDragEvent
    {
        get => onDragEvent;
        set => onDragEvent = value;
    }

    public UnityEvent<EventHandler, PointerEventData> OnDropEvent
    {
        get => onDropEvent;
        set => onDropEvent = value;
    }

    public UnityEvent<EventHandler, PointerEventData> OnEndDragEvent
    {
        get => onEndDragEvent;
        set => onEndDragEvent = value;
    }

    public UnityEvent<EventHandler, PointerEventData> OnPointerEnterEvent
    {
        get => onPointerEnterEvent;
        set => onPointerEnterEvent = value;
    }

    public UnityEvent<EventHandler, PointerEventData> OnPointerExitEvent
    {
        get => onPointerExitEvent;
        set => onPointerExitEvent = value;
    }

    public UnityEvent<EventHandler, PointerEventData> OnPointerClickEvent
    {
        get => onPointerClickEvent;
        set => onPointerClickEvent = value;
    }


    private bool isDragging = false;
    private bool isPointerFloating = false;
    public bool IsDragging => isDragging;
    public bool IsPointerFloating => isPointerFloating;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        onBeginDragEvent.Invoke(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDragEvent.Invoke(this, eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        onDropEvent.Invoke(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        onEndDragEvent.Invoke(this, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerFloating = true;
        onPointerEnterEvent.Invoke(this, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerFloating = false;
        onPointerExitEvent.Invoke(this, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onPointerClickEvent.Invoke(this, eventData);
    }
}
