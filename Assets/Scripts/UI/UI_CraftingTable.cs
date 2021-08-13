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
    private ItemCategory currentCategory;

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
            SetActive(!IsActive);
        }
    }

    public override void SetActive(bool _state)
    {
        if (_state)
        {
            currentSlot = -1;
            currentCategory = ItemCategory.None;
            blueprintInfo.alpha = 0;
            blueprintInfo.interactable = false;
            blueprintInfo.blocksRaycasts = false;
        }
        base.SetActive(_state);
    }

    public void OnClick_Category(int _category)
    {
        if (Enum.IsDefined(typeof(ItemCategory), _category))
        {
            currentCategory = (ItemCategory)_category;
            for(int i = 0; i < categoryList.Length; ++i)
            {
                categoryList[i].interactable = i != _category;
            }
        }
        else currentCategory = ItemCategory.None;
        MakeBlueprintList();
    }

    public void OnClick_Slot(int _slotIndex)
    {
        if (currentSlot != _slotIndex)
        {
            currentSlot = _slotIndex;
            Blueprint blueprint = blueprintData[currentSlot];
            MyItemData itemData = MyItemManager.Instance.GetItemData(blueprint.itemID);
            image_Thumbnail.sprite = MyItemManager.Instance.GetSprite(blueprint.itemID);
            if(blueprintInfo.alpha <= 0)
            {
                blueprintInfo.alpha = 1;
                blueprintInfo.interactable = true;
                blueprintInfo.blocksRaycasts = true;
            }
            text_ItemName.SetText(itemData.name);
            //text_ItemCount.SetText(아이템개수);
            text_ItemDescription.SetText(itemData.description);
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
                    needItemSlot[i].
                        SetActive(true).
                        SetName(MyItemManager.Instance.GetItemData(blueprint.needItems[i].id).name).
                        SetSprite(MyItemManager.Instance.GetSprite(blueprint.needItems[i].id));
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
            for (int i = 0; i < blueprint.needItems.Length; ++i)
            {
                break;
                if (Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id) < blueprint.needItems[i].count)
                {
                    canMake = false;
                    break;
                }
                // 여기서 제작가능한 아이템 개수 계산할 것.
            }
            if (canMake)
            {
                if (Keyboard.current[Key.LeftShift].IsPressed())
                {
                    UIManager.Instance.MakePopUp_Counter().
                        SetContent("제작 개수 선택").
                        SetValue(0, 999).onClick += (current, max) =>
                        {
                            UIManager.Instance.MakeNotice($"{MyItemManager.Instance.GetItemData(blueprint.itemID).name}x{current} 제작 완료.");
                        };
                }
                else
                {
                    UIManager.Instance.MakePopUp_Selection().
                        SetContent($"{MyItemManager.Instance.GetItemData(blueprint.itemID).name}x1\n제작하시겠습니까?").
                        SetSelection("확인", "취소").onClick += (index) =>
                        {
                            if(index == 0)
                            {
                                UIManager.Instance.MakeNotice($"{MyItemManager.Instance.GetItemData(blueprint.itemID).name}x1 제작 완료.");
                            }
                        };
                }
                return;
                for (int i = 0; i < blueprint.needItems.Length; ++i)
                {
                    Shelter.Instance.Inventory.TryUpdateItem(blueprint.needItems[i].id, -blueprint.needItems[i].count);
                }
                Shelter.Instance.Inventory.TryUpdateItem(blueprint.itemID, 1);
            }
            else
            {
                UIManager.Instance.MakeNotice("필요한 재료가 부족합니다.");
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
        Debug.Log("청사진 개수 : " + blueprintData.Count);
        for (int i = 0; i < blueprintData.Count; ++i)
        {
            UI_CraftingTableSlot newSlot = Instantiate(craftingTableSlot, blueprintSlotStorage);
            newSlot.onClick += OnClick_Slot;
            blueprintSlot.Add(
                newSlot.
                SetIndex(blueprintData[i].id).
                SetIcon(MyItemManager.Instance.GetSprite(blueprintData[i].itemID)));
        }
        MakeBlueprintList();
    }

    public void MakeBlueprintList()
    {
        for(int i = blueprintListContent.childCount - 1; i >= 0; --i)
        {
            blueprintListContent.GetChild(i).SetParent(blueprintSlotStorage);
        }

        List<Blueprint> currentData;
        if(currentCategory == ItemCategory.None)
        {
            currentData = blueprintData;   
        }
        else currentData = blueprintData.FindAll(x => x.category == currentCategory);

        for(int i = 0; i < currentData.Count; ++i)
        {
            blueprintSlot[currentData[i].id].SetParent(blueprintListContent);
        }
    }
}
