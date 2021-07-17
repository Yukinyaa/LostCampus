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
        // 여기서 거점 인벤토리를 저장된 파일로부터 불러와서 파싱한다.
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
                    // 새로운 아이템 슬롯이 추가되었을 때
                    // index = 추가된 아이템 슬롯의 index
                    // newItem = 추가된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_CLEAR:
                {
                    // 리스트가 초기화 되었을 때
                    UIManager.Instance.GetUI<UI_Storage>().ClearSlot();
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_INSERT:
                {
                    // 리스트 중간에 삽입되었을 때
                    // index = 추가된 아이템 슬롯의 index
                    // newItem = 추가된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_REMOVEAT:
                {
                    // 아이템 슬롯이 제거되었을 때
                    // index = 제거된 아이템 슬롯의 index
                    // oldItem = 제거된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_SET:
                {
                    // 기존 아이템 슬롯이 업데이트 되었을 때 (수치의 증감, 슬롯 교환 등)
                    // index = 업데이트 된 아이템 슬롯의 index
                    // oldItem = 업데이트 되기 이전의 아이템 슬롯
                    // newItem = 업데이트 된 아이템 슬롯
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
