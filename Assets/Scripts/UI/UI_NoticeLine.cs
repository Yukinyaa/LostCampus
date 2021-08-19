using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_NoticeLine : MonoBehaviour
{
    private RectTransform parent;
    private float lineHeight;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI text_Content;

    public CanvasGroup CanvasGroup
    {
        get
        {
            return canvasGroup;
        }
    }

    public UI_NoticeLine SetContent(string _content)
    {
        text_Content.SetText(_content);
        if (!string.IsNullOrEmpty(_content))
        {
            float stringWidth = text_Content.preferredWidth;
            float parentWidth = parent.rect.width;
            float preferredWidth = parentWidth - text_Content.margin.x - text_Content.margin.z;
            int lineCount = 1;
            while (stringWidth > preferredWidth)
            {
                lineCount++;
                stringWidth -= preferredWidth;
            }
            rectTransform.sizeDelta = new Vector2(lineCount > 1 ? parentWidth : stringWidth, lineHeight * lineCount);
        }
        return this;
    }

    public UI_NoticeLine SetParent(RectTransform _parent)
    {
        parent = _parent;
        rectTransform.SetParent(_parent);
        return this;
    }

    public UI_NoticeLine SetLineHeight(float _value)
    {
        lineHeight = _value;
        return this;
    }
}
