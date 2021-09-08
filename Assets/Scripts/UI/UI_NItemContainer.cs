using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NItemContainer : UIComponent
{
    [Header("UI ItemContainer")]
    [SerializeField] protected EventHandler eventHandler;
    protected List<UI_NItemSlot> itemSlot;

    public override void Init()
    {
        base.Init();
        itemSlot = new List<UI_NItemSlot>(GetComponentsInChildren<UI_NItemSlot>());
        for(int i = 0; i < itemSlot.Count; ++i)
        {
            itemSlot[i].SetIndex(i).SetContainer(this);
        }
    }
    protected override void OnHide()
    {
        base.OnHide();
        UI_NItemSlot.StopDrag(this);
    }
}
