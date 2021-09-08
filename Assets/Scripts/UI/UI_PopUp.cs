using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : UIComponent
{
    [Header("- PopUp")]
    [SerializeField] PopUp_ItemInfo popUp_ItemInfo;
    [SerializeField] PopUp_MiniMenu popUp_MiniMenu;
    [SerializeField] PopUp_Counter popUp_Counter;
    [SerializeField] PopUp_Selection popUp_Selection;

    public override void Init()
    {
        base.Init();
        popUp_Counter.Init();
        popUp_Selection.Init();
        popUp_MiniMenu.Init();
    }

    public void ShowItemInfo(ItemSlot _data, Vector3 _pos)
    {
        popUp_ItemInfo.
            SetName(_data.Name + (_data.Amount > 1 ? $"x{_data.Amount}" : string.Empty)).
            SetContent(_data.FlavorText).
            SetIcon(ItemInfoDataBase.GetSprite(_data.Sprite)).
            SetPosition(_pos).
            SetActive(true);
    }

    public void HideItemInfo()
    {
        popUp_ItemInfo.SetActive(false);
    }

    public PopUp_Counter MakePopUp_Counter()
    {
        return popUp_Counter.Clear();
    }

    public PopUp_Selection MakePopUp_Selection()
    {
        return popUp_Selection.Clear();
    }

    public PopUp_MiniMenu MakeMiniMenu(UIComponent _root)
    {
        return popUp_MiniMenu.Clear().SetRoot(_root);
    }

}
