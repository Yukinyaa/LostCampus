using System;
using System.Collections.Generic;
using Mirror;

public class MessageManager : NetworkBehaviour
{
    static TrafficChannel instance;
    public static TrafficChannel Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TrafficChannel>();

            return instance;
        }
    }

    private Queue<string> mailBoxQueue = new Queue<string>();

    public bool IsMail
    {
        get
        {
            return mailBoxQueue.Count > 0;
        }
    }

    [Client]
    public string GetMail()
    {
        try
        {
            return mailBoxQueue.Dequeue();
        }
        catch (InvalidOperationException)
        {
            return string.Empty;
        }
    }

    [Client]
    public void Send(string _message)
    {
        CmdSendMessage(_message);
    }

    [Command(requiresAuthority = false)]
    private void CmdSendMessage(string _message)
    {
        //여기서 message를 처리할 수 있다..
        //ClientRpc의 경우 모든 Client에게 메시지를 송신
        //TargetRpc의 경우 특정 Client에게 메시지를 송신 (귓속말 기능 수행 등)
        //차단 등의 기능을 구현한다고 치면 ClientRpc는 사용하지 않는 것이 좋아보임.
        RpcMakeLine($"{_message}");
    }

    [ClientRpc]
    private void RpcMakeLine(string _content)
    {
        mailBoxQueue.Enqueue(_content);
    }

}
