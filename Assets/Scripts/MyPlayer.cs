using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class MyPlayer : NetworkBehaviour
{
    [SerializeField]
    [SyncVar(hook = "OnNameChanged")]
    public string username = string.Empty;
    [SerializeField] private ThirdPersonController controller;
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
    
    [Client]
    public void LockCamera(Transform _transform)
    {
        controller.enabled = false;
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        Camera.main.fieldOfView = 60f;
        Camera.main.transform.SetPositionAndRotation(_transform.position, _transform.localRotation);
    }

    [Client]
    public void UnlockCameraPos()
    {
        controller.enabled = true;
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
    }
}
