using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShelterInventory : NetworkBehaviour
{
    [SerializeField]
    private SyncList<ItemSlot> _itemslots = new SyncList<ItemSlot>();
    public SyncList<ItemSlot> Itemslots
    {
        get => _itemslots;
    }

    public override void OnStartClient()
    {
        _itemslots.Callback += OnStorageUpdated;
        UIManager.Instance.GetUI<UI_Storage>().InitSlot(Itemslots);
    }

    public override void OnStartServer()
    {
        ItemSlot[] data = LoadInventoryData();
        for (int i = 0; i < data.Length; ++i)
        {
            _itemslots.Add(data[i]);
        }
        _itemslots.Callback += OnStorageUpdatedToServer;
    }

    private void OnStorageUpdated(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
    {
        switch (op)
        {
            case SyncList<ItemSlot>.Operation.OP_ADD:
                {
                    // ���ο� ������ ������ �߰��Ǿ��� ��
                    // index = �߰��� ������ ������ index
                    // newItem = �߰��� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_CLEAR:
                {
                    // ����Ʈ�� �ʱ�ȭ �Ǿ��� ��
                    UIManager.Instance.GetUI<UI_Storage>().ClearSlot();
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_INSERT:
                {
                    // ����Ʈ �߰��� ���ԵǾ��� ��
                    // index = �߰��� ������ ������ index
                    // newItem = �߰��� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().AddSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_REMOVEAT:
                {
                    // ������ ������ ���ŵǾ��� ��
                    // index = ���ŵ� ������ ������ index
                    // oldItem = ���ŵ� ������ ����
                    UIManager.Instance.GetUI<UI_Storage>().RemoveSlot(index, newItem);
                    break;
                }
            case SyncList<ItemSlot>.Operation.OP_SET:
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
    private void OnStorageUpdatedToServer(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldItem, ItemSlot newItem)
    {
        SaveInventoryData();
    }

    [Server]
    public void SaveInventoryData()
    {
        try
        {
            List<ItemSlot> storageData = new List<ItemSlot>(_itemslots.Count);
            _itemslots.CopyTo(storageData);
            string json = JsonHelper.ToJson(storageData.ToArray(), true);
            string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {

        }
    }

    [Server]
    public ItemSlot[] LoadInventoryData()
    {
        string path = Application.persistentDataPath + "/Server/" + "shelter.txt";
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonHelper.FromJson<ItemSlot>(json);
            }
            else
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Server/");
                ItemSlot[] newData = new ItemSlot[5];
                for (int i = 0; i < 5; ++i)
                {
                    newData[i] = new ItemSlot(ItemInfoDataBase.FindItemInfo(i), i);
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


}
