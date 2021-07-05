using System;
using System.Collections.Generic;
using Mirror;

public class TrafficChannel : NetworkBehaviour
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
        //���⼭ message�� ó���� �� �ִ�..
        //ClientRpc�� ��� ��� Client���� �޽����� �۽�
        //TargetRpc�� ��� Ư�� Client���� �޽����� �۽� (�ӼӸ� ��� ���� ��)
        //���� ���� ����� �����Ѵٰ� ġ�� ClientRpc�� ������� �ʴ� ���� ���ƺ���.
        RpcMakeLine($"{_message}");
    }

    [ClientRpc]
    private void RpcMakeLine(string _content)
    {
        mailBoxQueue.Enqueue(_content);
    }

}
