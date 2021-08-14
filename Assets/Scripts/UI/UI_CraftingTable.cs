using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftingTable : UIComponent
{
    private int currentSlot = -1;
    private ItemCategory currentCategory;

    private List<Blueprint> blueprintData;
    private List<UI_CraftingTableSlot> blueprintSlot;
    [SerializeField] private Transform blueprintListContent;
    [SerializeField] private Transform blueprintSlotStorage;

    [Header("- UI")]
    [SerializeField] private Image image_Thumbnail;
    [SerializeField] private TextMeshProUGUI text_ItemName;
    [SerializeField] private TextMeshProUGUI text_ItemCategory;
    [SerializeField] private TextMeshProUGUI text_ItemDescription;
    [SerializeField] private UI_NeedItemSlot[] needItemSlot;
    [Header("- Prefab")]
    [SerializeField] private UI_CraftingTableSlot craftingTableSlot;
    public void OnClick_Category(int _category)
    {
        if (Enum.IsDefined(typeof(ItemCategory), _category))
        {
            currentCategory = (ItemCategory)_category;
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
            text_ItemName.SetText(itemData.name);
            text_ItemCategory.SetText(itemData.category.ToString());
            text_ItemDescription.SetText(itemData.description);
            for (int i = 0; i < needItemSlot.Length; ++i)
            {
                if (i < blueprint.needItems.Length)
                {
                    needItemSlot[i].
                        SetActive(true).
                        SetSprite(MyItemManager.Instance.GetSprite(blueprint.needItems[i].id)).
                        SetCount(Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id), blueprint.needItems[i].count);
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
                if (Shelter.Instance.Inventory.GetCountByItemID(blueprint.needItems[i].id) < blueprint.needItems[i].count)
                {
                    canMake = false;
                    break;
                }
            }
            if (canMake)
            {
                for (int i = 0; i < blueprint.needItems.Length; ++i)
                {
                    Shelter.Instance.Inventory.TryUpdateItemById(blueprint.needItems[i].id, -blueprint.needItems[i].count);
                }
                Shelter.Instance.Inventory.TryUpdateItemById(blueprint.itemID, 1);
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
        for(int i = 0; i < blueprintData.Count; ++i)
        {
            UI_CraftingTableSlot newSlot = Instantiate(craftingTableSlot, blueprintListContent);
            newSlot.onClick += OnClick_Slot;
            blueprintSlot.Add(newSlot.
                SetIndex(blueprintData[i].id).
                SetName(MyItemManager.Instance.GetItemData(blueprintData[i].itemID).name).
                SetIcon(MyItemManager.Instance.GetSprite(blueprintData[i].itemID)));
        }
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
