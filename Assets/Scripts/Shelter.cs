using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public struct StorageSlot
{
    public int ID { get; private set; }
    public int count;

    public StorageSlot(int _id, int _count = 0)
    {
        ID = _id;
        count = _count;
    }
}
public class Shelter : NetworkBehaviour
{
    [SerializeField] private Transform shelterCameraAnchor;
    [SerializeField] private Transform[] restPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private bool isShelter = false;

    [SyncVar] private int playerCount = 0;
    public readonly SyncList<StorageSlot> storage = new SyncList<StorageSlot>();

    public override void OnStartClient()
    {
        storage.Callback += OnStorageUpdated;
        List<StorageSlot> data = new List<StorageSlot>(storage.Count);
        storage.CopyTo(data);
        UIManager.Instance.GetUI<UI_Storage>().InitSlot(data);
    }

    public override void OnStartServer()
    {
        // ���⼭ ���� �κ��丮�� ����� ���Ϸκ��� �ҷ��ͼ� �Ľ��Ѵ�.
        for(int i = 0; i < 50; ++i)
        {
            storage.Add(new StorageSlot(i, Random.Range(0, 999)));
        }
    }

    private void OnStorageUpdated(SyncList<StorageSlot>.Operation op, int index, StorageSlot oldItem, StorageSlot newItem)
    {
        switch (op)
        {
            case SyncList<StorageSlot>.Operation.OP_ADD:
                {
                    // ���ο� ������ ������ �߰��Ǿ��� ��
                    // index = �߰��� ������ ������ index
                    // newItem = �߰��� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_CLEAR:
                {
                    // ����Ʈ�� �ʱ�ȭ �Ǿ��� ��
                    UIManager.Instance.GetUI<UI_Storage>().ClearSlot();
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_INSERT:
                {
                    // ����Ʈ �߰��� ���ԵǾ��� ��
                    // index = �߰��� ������ ������ index
                    // newItem = �߰��� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_REMOVEAT:
                {
                    // ������ ������ ���ŵǾ��� ��
                    // index = ���ŵ� ������ ������ index
                    // oldItem = ���ŵ� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_SET:
                {
                    // ���� ������ ������ ������Ʈ �Ǿ��� �� (��ġ�� ����, ���� ��ȯ ��)
                    // index = ������Ʈ �� ������ ������ index
                    // oldItem = ������Ʈ �Ǳ� ������ ������ ����
                    // newItem = ������Ʈ �� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().UpdateSlot(index, oldItem, newItem);
                    break;
                }
        }
    }

    [TargetRpc]
    public void RpcJoinPlayerToShelter(NetworkConnection conn, MyPlayer _player)
    {
        JoinToShelter(_player);
    }

    [TargetRpc]
    public void RpcExitPlayerFromShelter(NetworkConnection conn, MyPlayer _player)
    {
        ExitFromShelter(_player);
    }

    [Client]
    public void JoinToShelter(MyPlayer _player)
    {
        isShelter = true;
        _player.LockCamera(shelterCameraAnchor);
        _player.transform.position = restPoint[playerCount % restPoint.Length].position;
        Physics.SyncTransforms();
        CmdJoinToShelter();
    }

    [Client]
    public void ExitFromShelter(MyPlayer _player)
    {
        isShelter = false;
        _player.transform.position = exitPoint.position;
        _player.UnlockCameraPos();
        Physics.SyncTransforms();
        CmdExitFromShelter();
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinToShelter()
    {
        playerCount++;
    }

    [Command(requiresAuthority = false)]
    public void CmdExitFromShelter()
    {
        playerCount--;
    }

    [Command(requiresAuthority = false)]
    public void CmdModifyInventory(int _slotIndex, int _value)
    {
        StorageSlot currentSlot = storage[_slotIndex];
        storage[_slotIndex] = new StorageSlot(currentSlot.ID, currentSlot.count + _value);
    }

    private void Update()
    {
        if (!isClient) return;
        if (!isShelter) return;

        if (!EventSystem.current.IsPointerOverGameObject() && Mouse.current.leftButton.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                switch (hit.transform.name)
                {
                    case "Armory":
                        UIManager.Instance.GetUI<UI_Armory>().SetActive(true);
                        break;
                    case "Chest":
                        UIManager.Instance.GetUI<UI_Storage>().SetActive(true);
                        break;
                    case "CraftingTable":
                        UIManager.Instance.GetUI<UI_CraftingTable>().SetActive(true);
                        break;
                    case "Door":
                        MessageManager.Instance.Send("/moveto Field");
                        break;
                }
            }
        }
    }


    /*
    [Client]
    void Input()
    {
        
    }*/
}
