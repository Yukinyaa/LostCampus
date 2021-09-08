using System;
using System.IO;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public enum ItemCategory
{
    None = 0,
    WEAPON,
    MATERIAL,
    HWEAPON
}

[Serializable]
public struct Blueprint
{
    public int id;
    public ItemCategory category;
    public int itemID;
    public StorageSlot[] needItems;
}

[Serializable]
public struct StorageSlot
{
    public int id;
    public int count;

    public StorageSlot(int _id, int _count = 0)
    {
        id = _id;
        count = _count;
    }
}
public class Shelter : NetworkBehaviour
{
    public static Shelter Instance { get; private set; } = null;

    [SerializeField] private Transform shelterCameraAnchor;
    [SerializeField] private Transform[] restPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private bool isShelter = false;

    public bool IsShelter => isShelter;

    [SerializeField] private ShelterInventory inventory;
    public ShelterInventory Inventory => inventory;

    [SyncVar] private int playerCount = 0;
    public readonly SyncList<Blueprint> blueprints = new SyncList<Blueprint>();

    public override void OnStartClient()
    {
        Instance = this;
        inventory = transform.GetComponent<ShelterInventory>();
        //List<Blueprint> blueprintData = new List<Blueprint>(blueprints.Count);
        //blueprints.CopyTo(blueprintData);
        //UIManager.Instance.GetUI<UI_CraftingTable>().InitBlueprint(blueprintData);
    }

    public override void OnStartServer()
    {
        inventory = transform.GetComponent<ShelterInventory>();
        Blueprint[] blueprintData = LoadBlueprintData();
        // for(int i = 0; i < blueprintData.Length; ++i)
        // {
        //     blueprints.Add(blueprintData[i]);
        // }
    }
    /*
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
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_CLEAR:
                {
                    // 리스트가 초기화 되었을 때
                    UIManager.Instance.GetUI<UI_Storage>().ClearSlot();
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(default);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_INSERT:
                {
                    // 리스트 중간에 삽입되었을 때
                    // index = 추가된 아이템 슬롯의 index
                    // newItem = 추가된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_REMOVEAT:
                {
                    // 아이템 슬롯이 제거되었을 때
                    // index = 제거된 아이템 슬롯의 index
                    // oldItem = 제거된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().RemoveSlot(index, newItem);
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_SET:
                {
                    // 기존 아이템 슬롯이 업데이트 되었을 때 (수치의 증감, 슬롯 교환 등)
                    // index = 업데이트 된 아이템 슬롯의 index
                    // oldItem = 업데이트 되기 이전의 아이템 슬롯
                    // newItem = 업데이트 된 아이템 슬롯
                    UIManager.Instance.GetUI<UI_Storage>().UpdateSlot(index, oldItem, newItem);
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
        }
    }*/

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

    [Server]
    public Blueprint[] LoadBlueprintData()
    {
        string path = Application.persistentDataPath + "/Server/" + "blueprint.txt";
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonHelper.FromJson<Blueprint>(json);
            }
            else return null;
        }
        catch (Exception)
        {
            return null;
        }
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
                        //UIManager.Instance.GetUI<UI_Armory>().SetActive(true);
                        break;
                    case "Chest":
                    case "CraftingTable":
                        inventory.ActiveContainerUI();
                        //UIManager.Instance.GetUI<UI_CraftingTable>().SetState(true);
                        break;
                    case "Door":
                        MessageManager.Instance.Send("/moveto Field");
                        break;
                }
            }
        }
    }
}
