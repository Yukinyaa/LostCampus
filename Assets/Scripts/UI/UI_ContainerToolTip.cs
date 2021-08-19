using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ContainerToolTip : UIComponent
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI instruction;
    [SerializeField] private Image icon;
    private RectTransform rect;

    public RectTransform Rect
    {
        get
        {
            if ((object)rect == null)
            {
                rect = transform.GetComponent<RectTransform>();
            }

            return rect;
        }
    }

    public void setToolTip(ItemSlot itemSlot, Vector3 position)
    {
        title.text = itemSlot.Name;
        instruction.text = itemSlot.FlavorText;
        icon.sprite = ItemInfoDataBase.GetSprite(itemSlot.Sprite);
        Vector3 bias = new Vector3(Rect.rect.width / 2, -Rect.rect.height / 2);
        transform.position = position + bias;
    }
}
