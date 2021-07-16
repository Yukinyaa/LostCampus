using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ShelterInventory
{
    public static int MAX_STORAGE = 2000;
    public class ShelterInventorySlot
    {
        private ShelterInventory inventory;
        public int ID { get; private set; }
        private int count;
        public int Count 
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                if(count <= 0)
                {
                    inventory.RemoveSlotByItemID(ID);
                }
            }
        }

        public ShelterInventorySlot(ShelterInventory _inventory, int _id, int _count = 0)
        {
            inventory = _inventory;
            ID = _id;
            Count = _count;
        }
    }

    private List<ShelterInventorySlot> slots;
    private Dictionary<int, int> itemIDToSlotID;

    public int currentStorage
    {
        get
        {
            return slots.Count;
        }
    }

    public ShelterInventory()
    {
        slots = new List<ShelterInventorySlot>(MAX_STORAGE);
        itemIDToSlotID = new Dictionary<int, int>();
    }

    public ShelterInventorySlot GetSlotBySlotID(int _id)
    {
        if(0 <= _id && _id < slots.Count)
        {
            return slots[_id];
        }
        return null;
    }

    public ShelterInventorySlot GetSlotByItemID(int _id)
    {
        if (HasItem(_id) == false)
        {
            ShelterInventorySlot newSlot = new ShelterInventorySlot(this, _id);
            slots.Add(newSlot);
            itemIDToSlotID.Add(_id, slots.LastIndexOf(newSlot));
        }
        return slots[itemIDToSlotID[_id]];
    }

    public bool RemoveSlotBySlotID(int _id)
    {
        if(0 <= _id && _id < slots.Count)
        {
            ShelterInventorySlot removeSlot = slots[_id];
            slots.RemoveAt(_id);
            itemIDToSlotID.Remove(removeSlot.ID);
            return true;
        }
        return false;
    }

    public bool RemoveSlotByItemID(int _id)
    {
        if (HasItem(_id))
        {
            slots.RemoveAt(itemIDToSlotID[_id]);
            itemIDToSlotID.Remove(_id);
            return true;
        }
        return false;
    }

    public bool HasItem(int _id)
    {
        return itemIDToSlotID.ContainsKey(_id);
    }

    public void SortItem()
    {
        // 만약 구현할 일이 있다면 인자값으로 아이템 이름 순, 아이템 개수 순, 레어도 순 등등.... 으로 넣어서 할 수 있지 않을까?
    }

    public string ToData()
    {
        // 이 string을 서버에 저장하고
        return string.Empty;
    }

    public static ShelterInventory Parse(string _data)
    {
        // 서버에선 저장된 string을 불러와서 초기화한다.
        return null;
    }
}

public class Shelter : NetworkBehaviour
{
    [SerializeField] private Transform shelterCameraAnchor;
    [SerializeField] private Transform restPoint;

    [SyncVar]
    private ShelterInventory shelterInventory;

    [SerializeField] private bool isShelter = false;

    public override void OnStartServer()
    {
        shelterInventory = new ShelterInventory();
    }

    [TargetRpc]
    public void RpcJoinPlayerToShelter(NetworkConnection conn, MyPlayer _player)
    {
        isShelter = true;
        _player.LockCamera(shelterCameraAnchor);
        _player.transform.position = restPoint.position;
    }

    [TargetRpc]
    public void RpcExitPlayerFromShelter(NetworkConnection conn, MyPlayer _player)
    {
        // 여기서 필드 선택 같은걸 하게 한 다음에 그 필드에 따라서 다른 위치로 이동시킨다.
        isShelter = false;
        _player.transform.position = Vector3.zero;
        _player.UnlockCameraPos();
       
    }

    private void Update()
    {
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
                    case "CraftingTable":
                        UIManager.Instance.GetUI<UI_CraftingTable>().SetActive(true);
                        break;
                    case "Door":
                        MessageManager.Instance.Send("/MoveTo Field");
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
