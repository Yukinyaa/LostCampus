using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Console : UIComponent
{
    private static int MAX_LINECOUNT = 100;

    private Queue<UI_ConsoleLine> consoleQueue;
    private Queue<UI_ConsoleLine> storageQueue;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Transform storage;
    [Header("- Prefab")]
    [SerializeField] private UI_ConsoleLine consoleLine;

    public override void Init()
    {
        consoleQueue = new Queue<UI_ConsoleLine>(MAX_LINECOUNT);
        storageQueue = new Queue<UI_ConsoleLine>(MAX_LINECOUNT);
        for (int lineIndex = 0; lineIndex < MAX_LINECOUNT; ++lineIndex)
        {
            storageQueue.Enqueue(
                Instantiate(consoleLine).
                SetParent(storage).
                SetContent(string.Empty));
        }
    }

    public void MakeLine(string _message)
    {
        if (consoleQueue.Count >= MAX_LINECOUNT)
        {
            storageQueue.Enqueue(consoleQueue.Dequeue().SetParent(storage).SetPosition(0));
        }
        consoleQueue.Enqueue(
            storageQueue.
            Dequeue().
            SetParent(content).
            SetContent(_message));
    }

    public void OnEndEdit()
    {
        //if (false == Input.GetKeyDown(KeyCode.Return)) return;
        if (string.IsNullOrWhiteSpace(inputField.text)) return;
        Send();
    }

    public void OnClick_Enter()
    {
        Send();
    }

    private void Send()
    {
        //CmdSendMessage(inputField.text);

        string text = inputField.text;

        if (text.Contains("SetTime"))
        {
            string[] str = text.Split(' ');
            float time;
            if(float.TryParse(str[1], out time))
            {
                Debug.Log("Time Change : " + time);
                TimeController.Instance.ChangeTime(time);
            }
        }
        else
        {
            TrafficChannel.Instance.Send(inputField.text);
        }
        inputField.SetTextWithoutNotify(string.Empty);
    }


    private void Update()
    {
        try
        {
            while (TrafficChannel.Instance.IsMail)
            {
                MakeLine(TrafficChannel.Instance.GetMail());
            }
        }
        catch (Exception)
        {

        }
    }
}
