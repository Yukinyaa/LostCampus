using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StorageSlot : MonoBehaviour
{
    public Action<int> onClick;
    public int slotIndex;
    public int itemIndex;
    [SerializeField] private Image image_Item;
    [SerializeField] private TextMeshProUGUI text_Count;

    public UI_StorageSlot SetIndex(int _index)
    {
        slotIndex = _index;
        return this;
    }

    public UI_StorageSlot SetSlot(StorageSlot _slotData)
    {
        itemIndex = _slotData.ID;
        SetCount(_slotData.count);
        return this;
    }

    public UI_StorageSlot SetImage(Sprite _sprite)
    {
        image_Item.sprite = _sprite;
        return this;
    }

    public UI_StorageSlot SetCount(int _count)
    {
        if (_count <= 0) text_Count.text = string.Empty;
        else text_Count.text = _count.ToString();
        return this;
    }

    public void OnClick()
    {
        onClick?.Invoke(slotIndex);
    }
}
