using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_NeedItemSlot : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private Image image_Item;
    [SerializeField] private TextMeshProUGUI text_Name;
    [SerializeField] private TextMeshProUGUI text_Count;

    public UI_NeedItemSlot SetActive(bool _state)
    {
        gameObject.SetActive(_state);
        return this;
    }

    public UI_NeedItemSlot SetName(string _name)
    {
        text_Name.SetText(_name);
        return this;
    }

    public UI_NeedItemSlot SetSprite(Sprite _sprite)
    {
        image_Item.sprite = _sprite;
        return this;
    }

    public UI_NeedItemSlot SetCount(int _current, int _max)
    {
        text_Count.SetText(_current < _max ? $"<color=red>{_current}</color> / {_max}" : $"{_current} / {_max}");
        return this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("마우스 오버");
    }
}
