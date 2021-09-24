using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInv : UI_NItemContainer
{
    [Header("UI Inventory")]
    [SerializeField] private RectTransform view;
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup contentLayoutGroup;

    [Header("- Prefab")]
    [SerializeField] private UI_NPlayerInvSlot playerInvSlot;

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

    private void Start()
    {
        for (int i = 0; i < 6; ++i)
        {
            itemSlot.Add(Instantiate(playerInvSlot, content).SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(i + 1), i + 5)).SetIndex(i).SetContainer(this));
        }
    }
}
