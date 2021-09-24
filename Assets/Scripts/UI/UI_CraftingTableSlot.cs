using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftingTableSlot : MonoBehaviour
{
    public int index;
    public Action<int> onClick;
    [SerializeField] private Image image_Icon;

    public UI_CraftingTableSlot SetIcon(Sprite _icon)
    {
        image_Icon.sprite = _icon;
        return this;
    }

    public UI_CraftingTableSlot SetIndex(int _index)
    {
        index = _index;
        return this;
    }

    public UI_CraftingTableSlot SetParent(Transform _parent)
    {
        transform.SetParent(_parent);
        return this;
    }

    public void OnClick_Slot()
    {
        onClick?.Invoke(index);
    }

}
