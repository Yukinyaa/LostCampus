using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = "OnNameChanged")]
    public string username = string.Empty;
    [Header("- UI")]
    [SerializeField]
    private UI_PlayerUI playerUI;

    [Client]
    public void SetName(string _content)
    {
        username = _content;
    }

    public override void OnStartAuthority()
    {
        if (hasAuthority)
        {
            // �밭 �������� �����δ� '�г���'�� ���ÿ� ���� -> ������ ������ �� �����ϰ�, ���Ƿ� �ٲ� �� ���� (��¥�� �α����� �������� ���״ϱ�..?)
            SetName(MySetting.UserName);
            MessageManager.Instance.JoinPlayer(this);
        }
    }

    private void OnNameChanged(string _oldName, string _newName)
    {
        playerUI.SetName(_newName);
    }
}
