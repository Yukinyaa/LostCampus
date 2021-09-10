using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_CraftingTable : UIComponent
{
    private int currentSlot = -1;
    private ItemType currentCategory;

    private List<Blueprint> blueprintData;
    private List<UI_CraftingTableSlot> blueprintSlot;
    [SerializeField] private Transform blueprintListContent;
    [SerializeField] private Transform blueprintSlotStorage;
    [Header("- UI")]
    [SerializeField] private CanvasGroup blueprintInfo;
    [SerializeField] private Image image_Thumbnail;
    [SerializeField] private TextMeshProUGUI text_ItemName;
    [SerializeField] private TextMeshProUGUI text_ItemCount;
    [SerializeField] private TextMeshProUGUI text_ItemDescription;
    [SerializeField] private UI_NeedItemSlot[] needItemSlot;
    [SerializeField] private Button[] categoryList;
    [Header("- Prefab")]
    [SerializeField] private UI_CraftingTableSlot craftingTableSlot;

    private void Start()
    {
        StartCoroutine(_Init());
        IEnumerator _Init()
        {
            yield return null;
            string path = Application.persistentDataPath + "/Server/" + "blueprint.txt";
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    Debug.Log(json);
                    InitBlueprint(new List<Blueprint>(JsonHelper.FromJson<Blueprint>(json)));
                }
            }
            catch (Exception)
            {

            }
        }
    }
    private void Update()
    {
        if (Keyboard.current[Key.C].wasPressedThisFrame)
        {
            SetState(!IsActive);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        currentSlot = -1;
        currentCategory = ItemType.None;
        blueprintInfo.alpha = 0;
        blueprintInfo.interactable = false;
        blueprintInfo.blocksRaycasts = false;
    }

    public void OnClick_Category(int _category)
    {
        if (Enum.IsDefined(typeof(ItemType), _category))
        {
            currentCategory = (ItemType)_category;
            for(int i = 0; i < categoryList.Length; ++i)
            {
                categoryList[i].interactable = i != _category;
            }
        }
        else currentCategory = ItemType.None;
        MakeBlueprintList();
    }

    public void OnClick_Slot(int _slotIndex)
    {
        if (currentSlot != _slotIndex)
        {
            currentSlot = _slotIndex;
            Blueprint blueprint = blueprintData[currentSlot];
            //MyItemData itemData = MyItemManager.Instance.GetItemData(blueprint.itemID);
            ItemInfo itemInfo = ItemInfoDataBase.FindItemInfo(blueprint.itemID);
            //image_Thumbnail.sprite = MyItemManager.Instance.GetSprite(blueprint.itemID);
            image_Thumbnail.sprite = ItemInfoDataBase.GetSprite(itemInfo.Sprite);
            if(blueprintInfo.alpha <= 0)
            {
                blueprintInfo.alpha = 1;
                blueprintInfo.interactable = true;
                blueprintInfo.blocksRaycasts = true;
            }
            text_ItemName.SetText(itemInfo.Name);
            //text_ItemCount.SetText(�����۰���);
            text_ItemDescription.SetText(itemInfo.FlavorText);
            for (int i = 0; i < needItemSlot.Length; ++i)
            {
                if (i < blueprint.needItems.Length)
                {
                    /*
                    needItemSlot[i].
                        SetActive(true).
                        SetSprite(MyItemManager.Instance.GetSprite(blueprint.needItems[i].id)).
                        SetCount(Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id), blueprint.needItems[i].count);
                    */

                    ItemInfo needItemInfo = ItemInfoDataBase.FindItemInfo(blueprint.needItems[i].id);
                    needItemSlot[i].
                        SetActive(true).
                        SetName(needItemInfo.Name).
                        SetCount(999, blueprint.needItems[i].count).
                        SetSprite(ItemInfoDataBase.GetSprite(needItemInfo.Sprite));
                }
                else needItemSlot[i].SetActive(false);
            }
        }
    }

    public void OnClick_Craft()
    {
        if (currentSlot >= 0)
        {
            Blueprint blueprint = blueprintData[currentSlot];
            bool canMake = true;
            int canMakeCount = 999;
            for (int i = 0; i < blueprint.needItems.Length; ++i)
            {
                break;
                if (Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id) < blueprint.needItems[i].count)
                {
                    canMake = false;
                    break;
                }
                // ���⼭ ���۰����� ������ ���� ����� ��.
            }
            if (canMake)
            {
                ItemInfo itemInfo = ItemInfoDataBase.FindItemInfo(blueprint.itemID);
                if (Keyboard.current[Key.LeftShift].IsPressed())
                {
                    UIManager.Instance.MakePopUp_Counter().
                        SetContent("���� ���� ����").
                        SetValue(1, 1, canMakeCount).onClick += (current, max) =>
                        {
                            UIManager.Instance.MakeNotice($"{itemInfo.Name}x{current} ���� �Ϸ�.");
                        };
                }
                else
                {
                    UIManager.Instance.MakePopUp_Selection().
                        SetContent($"{itemInfo.Name}x1\n�����Ͻðڽ��ϱ�?").
                        SetSelection("Ȯ��", "���").onClick += (index) =>
                        {
                            if(index == 0)
                            {
                                UIManager.Instance.MakeNotice($"{itemInfo.Name}x1 ���� �Ϸ�.");
                            }
                        };
                }
                return;
                for (int i = 0; i < blueprint.needItems.Length; ++i)
                {
                    Shelter.Instance.Inventory.TryUpdateItemById(blueprint.needItems[i].id, -blueprint.needItems[i].count);
                }
                Shelter.Instance.Inventory.TryUpdateItemById(blueprint.itemID, 1);
            }
            else
            {
                UIManager.Instance.MakeNotice("�ʿ��� ��ᰡ �����մϴ�.");
            }
        }
    }

    public void OnStorageChanged(StorageSlot _newSlot)
    {
        if (currentSlot >= 0)
        {
            Blueprint blueprint = blueprintData[currentSlot];
            for(int i = 0; i < blueprint.needItems.Length; ++i)
            {
                if(_newSlot.Equals(default))
                {
                    needItemSlot[i].SetCount(Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id), blueprint.needItems[i].count);
                }
                else
                {
                    if (blueprint.needItems[i].id == _newSlot.id)
                    {
                        needItemSlot[i].SetCount(Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id), blueprint.needItems[i].count);
                        break;
                    }
                }
            }
        }
    }

    public void InitBlueprint(List<Blueprint> _data)
    {
        blueprintData = _data;
        blueprintSlot = new List<UI_CraftingTableSlot>(blueprintData.Count);
        Debug.Log("û���� ���� : " + blueprintData.Count);
        for (int i = 0; i < blueprintData.Count; ++i)
        {
            UI_CraftingTableSlot newSlot = Instantiate(craftingTableSlot, blueprintSlotStorage);
            newSlot.onClick += OnClick_Slot;
            blueprintSlot.Add(
                newSlot.
                SetIndex(blueprintData[i].id).
                SetIcon(ItemInfoDataBase.GetSprite(ItemInfoDataBase.FindItemInfo(blueprintData[i].itemID).Sprite)));
        }
        MakeBlueprintList();
    }

    public void MakeBlueprintList()
    {
        for(int i = blueprintListContent.childCount - 1; i >= 0; --i)
        {
            blueprintListContent.GetChild(i).SetParent(blueprintSlotStorage);
        }
        Debug.Log(currentCategory);

        List<Blueprint> currentData;
        if(currentCategory == ItemType.None)
        {
            currentData = blueprintData;   
        }
        else currentData = blueprintData.FindAll(x => ItemInfoDataBase.FindItemInfo(x.itemID).Type == currentCategory);

        for(int i = 0; i < currentData.Count; ++i)
        {
            blueprintSlot[currentData[i].id].SetParent(blueprintListContent);
        }
    }
}