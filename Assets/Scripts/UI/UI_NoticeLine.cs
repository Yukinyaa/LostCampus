using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_NoticeLine : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI text_Content;

    public UI_NoticeLine SetContent(string _content)
    {
        text_Content.SetText(_content);
        rectTransform.sizeDelta = new Vector2(text_Content.preferredWidth, 40);
        return this;
    }

    public UI_NoticeLine SetParent(Transform _parent)
    {
        rectTransform.SetParent(_parent);
        return this;
    }

}
