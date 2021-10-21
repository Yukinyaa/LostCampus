using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_TopBar : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI text_MinMax;
    [SerializeField] private Button button_MinMax;
    [SerializeField] private Button button_Exit;
    [SerializeField] private UIComponent target;

    Vector2 plusVector;

    Vector2 screenLeftDownPos;
    Vector2 screenRightUpPos;

    private void Start()
    {
        if (target is null)
        {
            target = GetComponentInParent<UIComponent>();
        }
        target.onMinimize += () => text_MinMax.SetText("□");
        target.onMaximize += () => text_MinMax.SetText("_");
        target.onShow += () => text_MinMax.SetText(target.IsMini ? "□" : "_");
        button_Exit.onClick.AddListener(delegate { target.Hide(); });
        button_MinMax.onClick.AddListener(delegate
        {
            if (target.IsMini)
            {
                target.Maximize();
            }
            else
            {
                target.Minimize();
            }
        });
        screenLeftDownPos = Vector2.zero;
        screenRightUpPos = new Vector2(UIManager.Instance.RectTransform.rect.width, UIManager.Instance.RectTransform.rect.height);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (target.IsActive)
        {
            plusVector = new Vector2(target.transform.position.x, target.transform.position.y) - eventData.position;
            target.HideMiniMenu();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target.IsActive)
        {
            target.transform.position = eventData.position + plusVector;

            Vector3 compensateVector = Vector3.zero;
            Vector2 leftDownPos = new Vector2(
                rectTransform.position.x - rectTransform.rect.width * .5f,
                rectTransform.position.y - rectTransform.rect.height);
            Vector2 rightUpPos = new Vector2(
                rectTransform.position.x + rectTransform.rect.width * .5f,
                rectTransform.position.y);

            if (leftDownPos.x < screenLeftDownPos.x) compensateVector.x += screenLeftDownPos.x - leftDownPos.x;
            if (leftDownPos.y < screenLeftDownPos.y) compensateVector.y += screenLeftDownPos.y - leftDownPos.y;
            if (rightUpPos.x > screenRightUpPos.x) compensateVector.x -= rightUpPos.x - screenRightUpPos.x;
            if (rightUpPos.y > screenRightUpPos.y) compensateVector.y -= rightUpPos.y - screenRightUpPos.y;

            target.transform.position += compensateVector;
        }
    }
}
