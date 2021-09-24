using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp_ItemInfo : MonoBehaviour
{
    private static Vector2 DEFAULT_SIZE = new Vector2(300, 200);

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popUp;
    [SerializeField] private Image image_Item;
    [SerializeField] private TextMeshProUGUI text_Name;
    [SerializeField] private TextMeshProUGUI text_Content;

    public PopUp_ItemInfo SetPosition(Vector3 _pos)
    {
        popUp.anchoredPosition = _pos;
        return this;
    }

    public PopUp_ItemInfo SetName(string _content)
    {
        text_Name.SetText(_content);
        return this;
    }
    public PopUp_ItemInfo SetContent(string _content)
    {
        text_Content.SetText(_content);
        return this;
    }
    public PopUp_ItemInfo SetIcon(Sprite _content)
    {
        image_Item.sprite = _content;
        return this;
    }

    public PopUp_ItemInfo SetActive(bool _state)
    {
        canvasGroup.alpha = _state ? 1 : 0;
        return this;
    }
}
