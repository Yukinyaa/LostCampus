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
            // 대강 떠오르는 구조로는 '닉네임'은 로컬에 저장 -> 서버에 접속할 때 설정하고, 임의로 바꿀 수 있음 (어짜피 로그인은 스팀으로 할테니까..?)
            SetName(MySetting.UserName);
            MessageManager.Instance.JoinPlayer(this);
        }
    }

    private void OnNameChanged(string _oldName, string _newName)
    {
        playerUI.SetName(_newName);
    }
}
