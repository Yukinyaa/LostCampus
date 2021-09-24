using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : MonoBehaviour
{
    [Header("- PopUp")]
    [SerializeField] PopUp_ItemInfo popUp_ItemInfo;
    [SerializeField] PopUp_MiniMenu popUp_MiniMenu;
    [SerializeField] PopUp_Counter popUp_Counter;
    [SerializeField] PopUp_Selection popUp_Selection;

    public void ShowTooltip(ItemSlot _data, Vector3 _pos)
    {
        popUp_ItemInfo.
            SetName(_data.Name + (_data.Amount > 1 ? $"x{_data.Amount}" : string.Empty)).
            SetContent(_data.FlavorText).
            SetIcon(ItemInfoDataBase.GetSprite(_data.Sprite)).
            SetPosition(_pos).
            SetActive(true);
    }

    public void HideTooltip()
    {
        popUp_ItemInfo.SetActive(false);
    }

    public PopUp_Counter MakeCounter()
    {
        return popUp_Counter.Init();
    }

    public PopUp_Selection MakeSelection()
    {
        return popUp_Selection.Init();
    }

    public PopUp_MiniMenu MakeMiniMenu(UIComponent _root)
    {
        return popUp_MiniMenu.Init(_root);
    }

    public void HideMiniMenu()
    {
        popUp_MiniMenu.Kill();
    }

}
