using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipSlot : UI_NItemSlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (isDrag)
        {
            if (currentSlot != null)
            {
                if (itemCond.HasFlag(currentSlot.SlotData.Type))
                {
                    ItemSlot tempData = SlotData;
                    SlotData = currentSlot.SlotData;
                    if (tempData != null)
                    {
                        currentSlot.SlotData = tempData;
                        this.SetSlot(SlotData);
                    }
                    else isSwap = false;
                    UIManager.Instance.MakeNotice($"{currentSlot.SlotData.Name} ��� �����߽��ϴ�.");
                }
                else
                {
                    isSwap = false;
                    UIManager.Instance.MakeNotice("��� ������ �� �����ϴ�.");
                }
                isDrop = false;
            }
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (SlotData == null) return;
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                UIManager.Instance.MakeNotice("��� �����մϴ�.");
                break;
        }
    }
}
