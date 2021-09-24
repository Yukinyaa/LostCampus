using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;



public class UI_Console : UIComponent
{
    private static int MAX_LINECOUNT = 30;

    private Queue<UI_ConsoleLine> consoleQueue;
    private Queue<UI_ConsoleLine> storageQueue;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Transform storage;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private ScrollRect scrollRect;
    [Header("- Prefab")]
    [SerializeField] private UI_ConsoleLine consoleLine;

    public override void Init()
    {
        base.Init();
        consoleQueue = new Queue<UI_ConsoleLine>(MAX_LINECOUNT);
        storageQueue = new Queue<UI_ConsoleLine>(MAX_LINECOUNT);
        for (int lineIndex = 0; lineIndex < MAX_LINECOUNT; ++lineIndex)
        {
            storageQueue.Enqueue(
                Instantiate(consoleLine).
                SetParent(storage).
                SetContent(default));
        }
        inputField.onSubmit.AddListener(delegate { Send(); });
    }

    public void MakeLine(MessageManager.MessageData _data)
    {
        if (!_data.Equals(default(MessageManager.MessageData)))
        {
            if (consoleQueue.Count >= MAX_LINECOUNT)
            {
                UI_ConsoleLine oldLine = consoleQueue.
                    Dequeue().
                    SetParent(storage).
                    SetPosition(0);
                storageQueue.Enqueue(oldLine);
                scrollRect.content.sizeDelta = new Vector2(0, scrollRect.content.rect.height - oldLine.Height);
            }

            UI_ConsoleLine newLine = storageQueue.
                Dequeue().
                SetParent(scrollRect.content).
                SetContent(_data).
                SetPosition(-scrollRect.content.sizeDelta.y);
            consoleQueue.Enqueue(newLine);
            scrollRect.content.sizeDelta = new Vector2(0, scrollRect.content.rect.height + newLine.Height);

            if (scrollRect.content.rect.height > scrollRect.viewport.rect.height)
            {
                scrollbar.gameObject.SetActive(true);
                scrollRect.verticalScrollbar = scrollbar;
                scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
                scrollRect.viewport.offsetMin = new Vector2(scrollbar.GetComponent<RectTransform>().rect.width, scrollRect.viewport.offsetMin.y);
            }
            else
            {
                scrollRect.verticalScrollbar = null;
                scrollbar.gameObject.SetActive(false);
                scrollRect.viewport.offsetMin = new Vector2(0, scrollRect.viewport.offsetMin.y);
            }
        }
    }

    public void OnClick_Enter()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Send();
    }

    private void Send()
    {
        //CmdSendMessage(inputField.text);
        if (!string.IsNullOrEmpty(inputField.text))
        {
            MessageManager.Instance.Send(inputField.text);
            inputField.SetTextWithoutNotify(string.Empty);
        }
    }


    private void Update()
    {
        try
        {
            while (MessageManager.Instance.IsMail)
            {
                MakeLine(MessageManager.Instance.GetMail());
            }
        }
        catch (Exception)
        {

        }
    }
}
