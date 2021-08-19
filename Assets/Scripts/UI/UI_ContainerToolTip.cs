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
    private RectTransform rectTransform;

    public RectTransform RectTransform
    {
        get
        {
            if ((object)rectTransform == null)
            {
                rectTransform = transform.GetComponent<RectTransform>();
            }

            return rectTransform;
        }
    }

    public void setToolTip(ItemSlot itemSlot, Vector3 position)
    {
        title.text = itemSlot.Name;
        instruction.text = itemSlot.FlavorText;
        icon.sprite = ItemInfoDataBase.GetSprite(itemSlot.Sprite);
        Vector3 bias = new Vector3(RectTransform.rect.width / 2, -RectTransform.rect.height / 2);
        transform.position = position + bias;
    }
}
