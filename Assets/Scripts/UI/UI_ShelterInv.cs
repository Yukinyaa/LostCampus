using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShelterInv : UI_NItemContainer
{
    [Header("UI Inventory")]
    [SerializeField] private RectTransform view;
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup contentLayoutGroup;
    [SerializeField] private RectTransform hideContent;

    [Header("- Prefab")]
    [SerializeField] private UI_NPlayerInvSlot shelterInvSlot;

    public override void Init()
    {
        base.Init();
        float width =
            (view.rect.width
             - contentLayoutGroup.padding.left - contentLayoutGroup.padding.right
             - contentLayoutGroup.spacing.x * (contentLayoutGroup.constraintCount - 1))
            / contentLayoutGroup.constraintCount;

        contentLayoutGroup.cellSize = new Vector2(width, width);
    }

    public void SetItemFilter(ItemType _filter)
    {
        for(int i = 0; i < itemSlot.Count; ++i)
        {
            itemSlot[i].SetParent(_filter.HasFlag(itemSlot[i].SlotData.Type) ? content : hideContent);
        }
    }

    private void Start()
    {
        for (int i = 0; i < 6; ++i)
        {
            itemSlot.Add(Instantiate(shelterInvSlot, content).SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(i + 1), i + 5)).SetIndex(i).SetContainer(this));
        }
    }
}
