using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TopBar : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private UIComponent target;

    Vector2 plusVector;

    private void Start()
    {
        if (target == null)
        {
            target = GetComponentInParent<UIComponent>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (target.IsActive)
        {
            plusVector = new Vector2(target.transform.position.x, target.transform.position.y) - eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target.IsActive)
        {
            target.transform.position = eventData.position + plusVector;
        }
    }
}
