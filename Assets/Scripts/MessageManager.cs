using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mirror;

public class MessageManager : NetworkBehaviour
{
    public static string REGEX_CHECKUSERNAME = @"[^a-zA-Z0-9가-힣_]";
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

    [Server]
    public void SetUserName(MyPlayer _target, string _username)
    {
        string newName = _username;
        if(Regex.IsMatch(newName, REGEX_CHECKUSERNAME))
        {
            newName = Regex.Replace(newName, REGEX_CHECKUSERNAME, "");
        }
        if (string.IsNullOrEmpty(newName))
        {
            newName = "Player";
        }
        List<int> sameUserNameCount = new List<int>();
        foreach (MyPlayer connectedPlayer in players)
        {
            if (connectedPlayer == _target) continue;
            string[] words = connectedPlayer.username.Split(new char[] { '-' });
            if (words[0].Equals(newName))
            {
                if (words.Length > 1)
                {
                    if (int.TryParse(words[1], out int index))
                    {
                        sameUserNameCount.Add(index);
                    }
                }
                else sameUserNameCount.Add(0);
            }
        }
        if (sameUserNameCount.Count > 0)
        {
            int lastIndex = -1;
            sameUserNameCount.Sort();
            for (int index = 0; index < sameUserNameCount.Count; ++index)
            {
                if (index == sameUserNameCount[index])
                {
                    lastIndex = index;
                }
                else break;
            }
            if (lastIndex == -1) _target.username = newName;
            else _target.username =  $"{newName}-{lastIndex + 1}";
        }
        else _target.username = newName;
    }

    [Command(requiresAuthority = false)]
    private void CmdJoinPlayer(MyPlayer _newPlayer)
    {
        try
        {
            players.Add(_newPlayer);
            SetUserName(_newPlayer, _newPlayer.username);
            RpcSendAll(MessageType.Notice, $"{_newPlayer.username} Joined The Server");
            Shelter.Instance.RpcJoinPlayerToShelter(_newPlayer.netIdentity.connectionToClient, _newPlayer);
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
                        if(words.Length < 2 || string.IsNullOrEmpty(words[1]))
                        {
                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, $"/w /whisper (username[Str]) (message[Str])\nSend message to username");
                        }
                        else
                        {
                            try
                            {
                                string[] data = words[1].Split(new char[] { ' ' }, 2);
                                string target = data[0];
                                string content = data[1];
                                foreach (MyPlayer connectedPlayer in players)
                                {
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
                            }
                            catch (IndexOutOfRangeException)
                            {
                                RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Invalid Command");
                            }
                        }
                        break;
                    }
                case "SETTIME":
                    {
                        if(words.Length < 2 || string.IsNullOrEmpty(words[1]))
                        {
                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, $"/settime (time[Int])\nSet Time to time ( 1 ~ 24 )");
                        }
                        else
                        {
                            try
                            {
                                string[] data = words[1].Split(new char[] { ' ' }, 2);
                                float time;
                                if (float.TryParse(data[0], out time))
                                {
                                    RpcSendAll(MessageType.Notice, $"{_sender.username} Set Time To {time}");
                                    TimeController.Instance.SetTime(time);
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Invalid Command");
                            }
                        }
                        break;
                    }
                case "SETNAME":
                    {
                        if (words.Length < 2 || string.IsNullOrEmpty(words[1]))
                        {
                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, $"/setname (newName[Str])\nSet UserName to newName");
                        }
                        else
                        {
                            try
                            {
                                string newName = words[1];
                                if (Regex.IsMatch(newName, REGEX_CHECKUSERNAME))
                                {
                                    RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Invalid User Name : {newName}");
                                }
                                else
                                {
                                    string oldName = _sender.username;
                                    SetUserName(_sender, newName);
                                    RpcSendAll(MessageType.Notice, $"{oldName} Change Name To {_sender.username}");
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Invalid Command");
                            }
                        }
                        break;
                    }
                case "MOVETO":
                    {
                        if (words.Length < 2 || string.IsNullOrEmpty(words[1]))
                        {
                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, $"/moveto (place)\nMove To Current Place");
                        }
                        else
                        {
                            try
                            {
                                string place = words[1].ToUpper();
                                switch (place)
                                {
                                    case "HOME":
                                    case "SHELTER":
                                        {
                                            FindObjectOfType<Shelter>().RpcJoinPlayerToShelter(_sender.netIdentity.connectionToClient, _sender);
                                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, "Move To Shelter");
                                            break;
                                        }
                                    case "FIELD":
                                        {
                                            FindObjectOfType<Shelter>().RpcExitPlayerFromShelter(_sender.netIdentity.connectionToClient, _sender);
                                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, "Move To Field");
                                            break;
                                        }
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Invalid Command");
                            }
                        }
                        break;
                    }
                case "ADDITEM":
                    {
                        if (words.Length < 2 || string.IsNullOrEmpty(words[1]))
                        {
                            RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Notice, $"/AddItem (Place[Str]) (itemID[Hex]) (value[Int])\nAdd (itemID) Item To Certain Place (ex : Home, Inventory)");
                        }
                        else
                        {
                            try
                            {
                                string[] data = words[1].Split(new char[] { ' ' }, 3);
                                string place = data[0].ToUpper();
                                int itemID = data[1].Contains("0x") ? Convert.ToInt32(data[1], 16) : int.Parse(data[1], System.Globalization.NumberStyles.HexNumber);
                                int value = Convert.ToInt32(data[2]);
                                switch (place)
                                {
                                    case "HOME":
                                    case "SHELTER":
                                        {
                                            FindObjectOfType<Shelter>().Inventory.TryUpdateItemById(itemID, value);
                                            break;
                                        }
                                    case "INVEN":
                                    case "INVENTORY":
                                        {
                                            break;
                                        }
                                }
                            }
                            catch (IndexOutOfRangeException)
                            {
                                RpcSendMessage(_sender.netIdentity.connectionToClient, MessageType.Error, $"Invalid Command");
                            }
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
