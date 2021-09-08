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
                    // ���ο� ������ ������ �߰��Ǿ��� ��
                    // index = �߰��� ������ ������ index
                    // newItem = �߰��� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_CLEAR:
                {
                    // ����Ʈ�� �ʱ�ȭ �Ǿ��� ��
                    UIManager.Instance.GetUI<UI_Storage>().ClearSlot();
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(default);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_INSERT:
                {
                    // ����Ʈ �߰��� ���ԵǾ��� ��
                    // index = �߰��� ������ ������ index
                    // newItem = �߰��� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_REMOVEAT:
                {
                    // ������ ������ ���ŵǾ��� ��
                    // index = ���ŵ� ������ ������ index
                    // oldItem = ���ŵ� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().RemoveSlot(index, newItem);
                    UIManager.Instance.GetUI<UI_CraftingTable>().OnStorageChanged(newItem);
                    break;
                }
            case SyncList<StorageSlot>.Operation.OP_SET:
                {
                    // ���� ������ ������ ������Ʈ �Ǿ��� �� (��ġ�� ����, ���� ��ȯ ��)
                    // index = ������Ʈ �� ������ ������ index
                    // oldItem = ������Ʈ �Ǳ� ������ ������ ����
                    // newItem = ������Ʈ �� ������ ����
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
