using System;
using System.Collections.Generic;
using Mirror;

public class TrafficChannel : NetworkBehaviour
{
    private Queue<string> messageQueue;
    private static Queue<string> mailBoxQueue;

    public bool IsMail
    {
        get
        {
            return mailBoxQueue.Count > 0;
        }
    }


    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            UIManager.Instance.GetUI<UI_Console>().SetChannel(this);
            messageQueue = new Queue<string>();
            mailBoxQueue = new Queue<string>();
            messageQueue.Enqueue("Somebody Joined The Server.");
        }
        else
        {
            GetComponent<TrafficChannel>().enabled = false;
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
        messageQueue.Enqueue(_message);
    }

    [Command]
    private void CmdSendMessage(string _message)
    {
        //���⼭ message�� ó���� �� �ִ�..
        //ClientRpc�� ��� ��� Client���� �޽����� �۽�
        //TargetRpc�� ��� Ư�� Client���� �޽����� �۽� (�ӼӸ� ��� ���� ��)
        //���� ���� ����� �����Ѵٰ� ġ�� ClientRpc�� ������� �ʴ� ���� ���ƺ���.
        RpcMakeLine($"[{connectionToClient.connectionId}] : {_message}");
        
    }

    [ClientRpc]
    private void RpcMakeLine(string _content)
    {
        
        mailBoxQueue.Enqueue(_content);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        if(messageQueue.Count > 0)
        {
            CmdSendMessage(messageQueue.Dequeue());
        }
    }

}
