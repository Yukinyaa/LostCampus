using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDataContainer : MonoBehaviour
{
    public static ItemDataContainer container;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image image_Item;
    [SerializeField] private TextMeshProUGUI text_Count;
    public StorageSlot itemData;

    public ItemDataContainer SetSprite(Sprite _sprite)
    {
        image_Item.sprite = _sprite;
        return this;
    }

    public ItemDataContainer SetCount(int _count)
    {
        text_Count.SetText(_count.ToString());
        return this;
    }

    public ItemDataContainer SetData(StorageSlot _data)
    {
        itemData = _data;
        image_Item.sprite = MyItemManager.Instance.GetSprite(itemData.id);
        SetCount(itemData.count);
        return SetActive(itemData.count > 0);
    }

    public ItemDataContainer SetActive(bool _status)
    {
        canvasGroup.alpha = _status ? 1 : 0;
        return this;
    }

    public ItemDataContainer SetParent(Transform _parent)
    {
        transform.SetParent(_parent, true);
        return this;
    }
}
