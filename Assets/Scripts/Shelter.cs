using System;
using System.IO;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public enum BlueprintCategory
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
    public BlueprintCategory category;
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

    [SyncVar] private int playerCount = 0;
    public readonly SyncList<StorageSlot> storage = new SyncList<StorageSlot>();
    public readonly SyncList<Blueprint> blueprints = new SyncList<Blueprint>();

    public override void OnStartClient()
    {
        Instance = this;
        storage.Callback += OnStorageUpdated;
        List<StorageSlot> itemData = new List<StorageSlot>(storage.Count);
        List<Blueprint> blueprintData = new List<Blueprint>(blueprints.Count);
        storage.CopyTo(itemData);
        blueprints.CopyTo(blueprintData);
        UIManager.Instance.GetUI<UI_Storage>().InitSlot(itemData);
        UIManager.Instance.GetUI<UI_CraftingTable>().InitBlueprint(blueprintData);
    }

    public override void OnStartServer()
    {
        StorageSlot[] data = LoadInventoryData();
        for(int i = 0; i < data.Length; ++i)
        {
            storage.Add(data[i]);
        }

        Blueprint[] blueprintData = LoadBlueprintData();
        for(int i = 0; i < blueprintData.Length; ++i)
        {
            blueprints.Add(blueprintData[i]);
        }

        storage.Callback += OnStorageUpdatedToServer;
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

    private void OnStorageUpdatedToServer(SyncList<StorageSlot>.Operation op, int index, StorageSlot oldItem, StorageSlot newItem)
    {
        SaveInventoryData();
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
    
    [Client]
    public StorageSlot GetItemDataBySlotID(int _slotID)
    {
        if (0 <= _slotID && _slotID < storage.Count)
        {
            return storage[_slotID];
        }
        return default;
    }

    [Client]
    public List<StorageSlot> GetItemDataByItemID(int _itemID)
    {
        return storage.FindAll(x => x.id == _itemID);
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
    public void CmdModifyInventoryBySlotID(int _slotIndex, int _value)
    {
        ModifyInventoryBySlotID(_slotIndex, _value);
    }

    [Command(requiresAuthority = false)]
    public void CmdModifyInventoryByItemID(int _itemIndex, int _value)
    {
        ModifyInventoryByItemID(_itemIndex, _value);
    }

    [Server]
    public void ModifyInventoryBySlotID(int _slotIndex, int _value)
    {
        if(0 <= _slotIndex && _slotIndex < storage.Count)
        {
            StorageSlot currentSlot = storage[_slotIndex];
            if(currentSlot.count + _value > 0)
            {
                storage[_slotIndex] = new StorageSlot(currentSlot.id, currentSlot.count + _value);
            }
            else
            {
                storage.RemoveAt(_slotIndex);
            }
        }
    }

    [Server]
    public void ModifyInventoryByItemID(int _itemIndex, int _value)
    {
        int slotIndex = storage.FindIndex(x => x.id == _itemIndex);
        if (slotIndex == -1)
        {
            if (_value > 0)
            {
                StorageSlot newSlot = new StorageSlot(_itemIndex, _value);
                storage.Add(newSlot);
            }
        }
        else
        {
            ModifyInventoryBySlotID(slotIndex, _value);
        }
    }

    [Server]
    public void SaveInventoryData()
    {
        try
        {
            List<StorageSlot> storageData = new List<StorageSlot>(storage.Count);
            storage.CopyTo(storageData);
            string json = JsonHelper.ToJson(storageData.ToArray(), true);
            string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {

        }
    }

    [Server]
    public StorageSlot[] LoadInventoryData()
    {
        string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonHelper.FromJson<StorageSlot>(json);
            }
            else
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Server/");
                StorageSlot[] newData = new StorageSlot[30];
                for (int i = 0; i < 30; ++i)
                {
                    newData[i] = new StorageSlot(i, i + 1);
                }
                File.WriteAllText(path, JsonHelper.ToJson(newData, true));
                return newData;
            }
        }
        catch (Exception)
        {
            return null;
        }
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
}
