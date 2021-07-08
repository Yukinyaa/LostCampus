using System;
using System.Collections.Generic;
using Mirror;

public class MessageManager : NetworkBehaviour
{
    public enum MessageType
    {
        None = 0,
        Chat,
        Notice,
        Whisper,
        Error
    }

    public struct MessageData
    {
        public MessageType messageType;
        public string content;

        public MessageData(MessageType _type, string _content)
        {
            messageType = _type;
            content = _content;
        }
    }

    static MessageManager instance;
    public static MessageManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MessageManager>();

            return instance;
        }
    }

    public MyPlayer player;
    public HashSet<MyPlayer> players = new HashSet<MyPlayer>();


    private Queue<MessageData> messageQueue = new Queue<MessageData>();

    public bool IsMail
    {
        get
        {
            return messageQueue.Count > 0;
        }
    }

    [Client]
    public MessageData GetMail()
    {
        try
        {
            return messageQueue.Dequeue();
        }
        catch (InvalidOperationException)
        {
            return default;
        }
    }

    [Client]
    public void Send(string _message)
    {
        CmdSendMessage(player, _message);
    }

    [Client]
    public void JoinPlayer(MyPlayer _newPlayer)
    {
        player = _newPlayer;
        CmdJoinPlayer(_newPlayer);
    }

    [Command(requiresAuthority = false)]
    private void CmdJoinPlayer(MyPlayer _newPlayer)
    {
        try
        {
            players.Add(_newPlayer);
            RpcSendAll(MessageType.Notice, $"{_newPlayer.username} Joined The Server");
            print($"{_newPlayer.username} 이 서버에 입장.");
        }
        catch (Exception)
        {
            print($"{_newPlayer.username} 는 이미 존재하는 플레이어입니다.");
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSendMessage(MyPlayer _sender, string _message)
    {
        //여기서 message를 처리할 수 있다..
        //ClientRpc의 경우 모든 Client에게 메시지를 송신
        //TargetRpc의 경우 특정 Client에게 메시지를 송신 (귓속말 기능 수행 등)
        //차단 등의 기능을 구현한다고 치면 ClientRpc는 사용하지 않는 것이 좋아보임.
        if (string.IsNullOrEmpty(_message)) return;
        if (_message.TrimStart().StartsWith("/"))
        {
            string[] words = _message.TrimStart().Split(new char[] { ' ' }, 2);
            switch (words[0].TrimStart('/').ToUpper())
            {
                case "W":
                case "WHISPER":
                    {
                        string[] data = words[1].Split(new char[] { ' ' }, 2);
                        string target = data[0];
                        string content = data[1];
                        foreach(MyPlayer connectedPlayer in players)
                        {
                            print("Check Player List : " + connectedPlayer.username);

                            if (connectedPlayer.username.Equals(target))
                            {
                                RpcSendMessage(connectedPlayer.netIdentity.connectionToClient,
                                    MessageType.Whisper, $"From {_sender.username} : {content}");
                                RpcSendMessage(_sender.netIdentity.connectionToClient,
                                    MessageType.Whisper, $"To {target} : {content}");
                                return;
                            }
                        }
                        RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Wrong User Name : {target}");
                        break;
                    }
                case "SETTIME":
                    {
                        string[] data = words[1].Split(new char[] { ' ' }, 2);
                        float time;
                        if (float.TryParse(data[0], out time))
                        {
                            RpcSendAll(MessageType.Notice, $"{_sender.username} Set Time To {time}");
                            TimeController.Instance.SetTime(time);
                        }
                        break;
                    }
                case "SETNAME":
                    {
                        string[] data = words[1].Split(new char[] { ' ' }, 2);
                        string newName = data[0];
                        if (!string.IsNullOrEmpty(newName))
                        {
                            RpcSendAll(MessageType.Notice, $"{_sender.username} Change Name To {newName}");
                            _sender.username = newName;
                        }
                        break;
                    }
            }
        }
        else
        {
            RpcSendAll(MessageType.Chat, $"{_sender.username}:{_message}");
        }
    }

    [ClientRpc]
    private void RpcSendAll(MessageType _type, string _content)
    {
        messageQueue.Enqueue(new MessageData(_type, _content));
    }

    [TargetRpc]
    private void RpcSendMessage(NetworkConnection _target, MessageType _type, string _content)
    {
        messageQueue.Enqueue(new MessageData(_type, _content));
    }

}
