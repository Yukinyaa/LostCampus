using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UI_Button : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image image_Content;
    [SerializeField] private TextMeshProUGUI text_Content;

    public UI_Button SetText(string _content)
    {
        if((object)text_Content != null)
        {
            text_Content.SetText(_content);
        }
        return this;
    }

    public UI_Button SetSprite(Sprite _content)
    {
        if((object)image_Content != null)
        {
            image_Content.sprite = _content;
        }
        return this;
    }

    public UI_Button SetActive(bool _state)
    {
        gameObject.SetActive(_state);
        return this;
    }

    public UI_Button SetParent(Transform _parent)
    {
        transform.SetParent(_parent);
        return this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                button.onClick.Invoke();
                break;
        }
    }
}
