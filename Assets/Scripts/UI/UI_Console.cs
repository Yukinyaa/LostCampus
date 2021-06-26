using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UI_Console : NetworkBehaviour
{
    [SerializeField] private GameObject consoleUI;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform storage;
    [Header("- Prefab")]
    [SerializeField] private UI_ConsoleLine consoleLine;

    private static int MAX_LINECOUNT = 20;
    private static event Action<string> OnMessage;

    private Queue<UI_ConsoleLine> consoleQueue;
    private Queue<UI_ConsoleLine> storageQueue;
   

    public override void OnStartAuthority()
    {
        consoleUI.SetActive(true);
        consoleQueue = new Queue<UI_ConsoleLine>(MAX_LINECOUNT);
        storageQueue = new Queue<UI_ConsoleLine>(MAX_LINECOUNT);
        for (int lineIndex = 0; lineIndex < MAX_LINECOUNT; ++lineIndex)
        {
            storageQueue.Enqueue(
                Instantiate(consoleLine).
                SetParent(storage).
                SetContent(string.Empty));
        }
        OnMessage += MakeLine;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (false == hasAuthority) return;
        OnMessage -= MakeLine;
    }

    private void MakeLine(string _message)
    {
        if(consoleQueue.Count >= MAX_LINECOUNT)
        {
            storageQueue.Enqueue(consoleQueue.Dequeue().SetParent(storage).SetPosition(0));
        }
        consoleQueue.Enqueue(
            storageQueue.
            Dequeue().
            SetParent(content).
            SetContent(_message));
    }

    [Client]
    public void OnEndEdit()
    {
        if (false == Input.GetKeyDown(KeyCode.Return)) return;
        if (string.IsNullOrWhiteSpace(inputField.text)) return;
        Send();
    }

    [Client]
    public void Send()
    {
        CmdSendMessage(inputField.text);
        //inputField.SetTextWithoutNotify(string.Empty);
        inputField.SetTextWithoutNotify(string.Empty);
    }

    [Command]
    private void CmdSendMessage(string _message)
    {
        //여기서 meaage를 처리할 수 있다..
        //ClientRpc의 경우 모든 Client에게 메시지를 송신
        //TargetRpc의 경우 특정 Client에게 메시지를 송신 (귓속말 기능 수행 등)
        //차단 등의 기능을 구현한다고 치면 ClientRpc는 사용하지 않는 것이 좋아보임.
        RpcMakeLine($"[{connectionToClient.connectionId}] : {_message}");
    }

    [ClientRpc]
    private void RpcMakeLine(string _content)
    {
        OnMessage?.Invoke(_content);
    }

}
