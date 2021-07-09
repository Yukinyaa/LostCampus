using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ConsoleLine : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI text_Content;
    public float Height
    {
        get
        {
            return rectTransform.sizeDelta.y;
        }
    }

    public UI_ConsoleLine SetContent(MessageManager.MessageData _data)
    {
        text_Content.SetText(_data.content);
        switch (_data.messageType)
        {
            case MessageManager.MessageType.Chat:
                text_Content.color = Color.green;
                break;
            case MessageManager.MessageType.Notice:
                text_Content.color = Color.grey;
                break;
            case MessageManager.MessageType.Whisper:
                text_Content.color = Color.cyan;
                break;
            case MessageManager.MessageType.Error:
                text_Content.color = Color.red;
                break;
        }
        rectTransform.offsetMin = new Vector2(0, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(0, rectTransform.offsetMax.y);
        rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, text_Content.preferredHeight);
        return this;
    }

    public UI_ConsoleLine SetParent(Transform _transform)
    {
        rectTransform.SetParent(_transform);
        return this;
    }

    public UI_ConsoleLine SetPosition(float _y)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, _y);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        return this;
    }
}
