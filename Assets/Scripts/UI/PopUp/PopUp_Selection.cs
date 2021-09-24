using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp_Selection : MonoBehaviour
{
    private static Vector2 DEFAULT_SIZE = new Vector2(600, 300);

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popUp;
    [SerializeField] private TextMeshProUGUI text_Content;

    [SerializeField] private RectTransform content;
    [Header("- Selection")]
    [SerializeField] private Button[] selection;
    private List<TextMeshProUGUI> selectionText;

    public System.Action<int> onClick;

    private void Awake()
    {
        popUp.sizeDelta = DEFAULT_SIZE;
        selectionText = new List<TextMeshProUGUI>(selection.Length);
        for (int i = 0; i < selection.Length; ++i)
        {
            int selectionIndex = i;
            selection[selectionIndex].onClick.AddListener(
                delegate
                {
                    SetActive(false);
                    if (onClick != null)
                    {
                        System.Action<int> popupEvent = new System.Action<int>(onClick);
                        popupEvent?.Invoke(selectionIndex);
                    }
                });
            selectionText.Add(selection[selectionIndex].GetComponentInChildren<TextMeshProUGUI>());
            selection[i].gameObject.SetActive(false);
        }
    }

    public PopUp_Selection SetContent(string _content)
    {
        text_Content.SetText(_content);
        return this;
    }

    private PopUp_Selection SetActive(bool _state)
    {
        canvasGroup.alpha = _state ? 1 : 0;
        canvasGroup.interactable = _state;
        canvasGroup.blocksRaycasts = _state;
        return this;
    }

    public PopUp_Selection SetSelection(params string[] _contents)
    {
        for(int i = 0; i < selection.Length; ++i)
        {
            if (i < _contents.Length)
            {
                selection[i].gameObject.SetActive(true);
                selectionText[i].SetText(_contents[i]);
            }
        }
        Canvas.ForceUpdateCanvases();
        if (content.rect.width > DEFAULT_SIZE.x)
        {
            popUp.sizeDelta = new Vector2(content.rect.width + 20, DEFAULT_SIZE.y);
        }
        else
        {
            popUp.sizeDelta = DEFAULT_SIZE;
        }
        return this;
    }

    public PopUp_Selection Init()
    {
        onClick = null;
        for(int i = 0; i < selection.Length; ++i)
        {
            selection[i].gameObject.SetActive(false);
            selectionText[i].SetText(string.Empty);
        }
        return SetContent(string.Empty).SetActive(true);
    }


}
