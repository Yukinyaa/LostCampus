using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp_MiniMenu : MonoBehaviour
{
    private static Vector2 DEFAULT_SIZE = new Vector2(100, 20);

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popUp;
    [SerializeField] private RectTransform content;

    [Header("- Selection")]
    [SerializeField] private Button[] selection;
    private List<TextMeshProUGUI> selectionText;
    private UIComponent root;

    public System.Action<int> onClick;

    public void Init()
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
                    root.onBlur -= OnBlur;
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

    private PopUp_MiniMenu SetActive(bool _state)
    {
        canvasGroup.alpha = _state ? 1 : 0;
        canvasGroup.interactable = _state;
        canvasGroup.blocksRaycasts = _state;
        return this;
    }

    private void OnBlur()
    {
        SetActive(false);
        root.onBlur -= OnBlur;
    }

    public PopUp_MiniMenu SetRoot(UIComponent _root)
    {
        root = _root;
        root.onBlur += OnBlur;
        return this;
    }

    public PopUp_MiniMenu SetPosition(Vector3 _pos)
    {
        popUp.anchoredPosition = new Vector2(_pos.x - 10, _pos.y + 10);
        return this;
    }

    public PopUp_MiniMenu SetSelection(params string[] _contents)
    {
        for (int i = 0; i < selection.Length; ++i)
        {
            if (i < _contents.Length)
            {
                selection[i].gameObject.SetActive(true);
                selectionText[i].SetText(_contents[i]);
            }
        }
        Canvas.ForceUpdateCanvases();
        if (content.rect.height > DEFAULT_SIZE.y)
        {
            popUp.sizeDelta = new Vector2(DEFAULT_SIZE.x, content.rect.height);
        }
        else
        {
            popUp.sizeDelta = DEFAULT_SIZE;
        }
        return this;
    }

    public PopUp_MiniMenu Clear()
    {
        onClick = null;
        for (int i = 0; i < selection.Length; ++i)
        {
            selection[i].gameObject.SetActive(false);
            selectionText[i].SetText(string.Empty);
        }
        return SetActive(true);
    }
}
