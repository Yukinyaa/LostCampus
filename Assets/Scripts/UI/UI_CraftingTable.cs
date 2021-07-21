using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftingTable : UIComponent
{
    private int currentSlot;
    private BlueprintCategory currentCategory;

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
        if (Enum.IsDefined(typeof(BlueprintCategory), _category))
        {
            currentCategory = (BlueprintCategory)_category;
        }
        else currentCategory = BlueprintCategory.None;
        MakeBlueprintList();
    }

    public void OnClick_Slot(int _slotIndex)
    {
        currentSlot = _slotIndex;
        Blueprint blueprint = blueprintData[currentSlot];
        text_ItemName.SetText(blueprint.itemID.ToString());
        text_ItemCategory.SetText(blueprint.category.ToString());
        text_ItemDescription.SetText(string.Empty);
        for(int i = 0; i < needItemSlot.Length; ++i)
        {
            if (i < blueprint.needItems.Length)
            {
                int currentItemCount = 0;
                List<StorageSlot> currentItemData = Shelter.Instance.GetItemDataByItemID(blueprint.needItems[i].id);
                for (int j = 0; j < currentItemData.Count; ++j)
                {
                    currentItemCount += currentItemData[j].count;
                }
                needItemSlot[i].SetActive(true).SetSprite(null).SetCount(currentItemCount, blueprint.needItems[i].count);
            }
            else needItemSlot[i].SetActive(false);
        }
    }

    public void OnClick_Make()
    {

    }

    public void InitBlueprint(List<Blueprint> _data)
    {
        blueprintData = _data;
        blueprintSlot = new List<UI_CraftingTableSlot>(blueprintData.Count);
        for(int i = 0; i < blueprintData.Count; ++i)
        {
            UI_CraftingTableSlot newSlot = Instantiate(craftingTableSlot, blueprintListContent);
            newSlot.onClick += OnClick_Slot;
            blueprintSlot.Add(newSlot.SetIndex(blueprintData[i].id).SetName(blueprintData[i].itemID.ToString()));
        }
    }

    public void MakeBlueprintList()
    {
        for(int i = blueprintListContent.childCount - 1; i >= 0; --i)
        {
            blueprintListContent.GetChild(i).SetParent(blueprintSlotStorage);
        }

        List<Blueprint> currentData;
        if(currentCategory == BlueprintCategory.None)
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
