using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Equip : UI_NItemContainer
{
    [Header("- Equip")]
    [SerializeField] private RawImage equipImage;

    [Header("- Character Info")]
    [SerializeField] private RectTransform charInfo;
    [SerializeField] private TextMeshProUGUI charInfoText;
    [SerializeField] private Button charInfoButton;
    [SerializeField] private Image charInfoButtonImage;
    [SerializeField] private float charInfoAnimationTime;

    
    private bool charInfoState = false;
    private Sequence charInfoAnimation;
    private Vector2 charInfoDefaultPos;

    public override void Init()
    {
        base.Init();
        eventHandler.OnDropEvent.AddListener(delegate { UI_NItemSlot.StopDrag(this); });
        charInfoDefaultPos = charInfo.anchoredPosition;
        charInfo.anchoredPosition = new Vector2(charInfoDefaultPos.x - charInfo.sizeDelta.x, charInfoDefaultPos.y);
        charInfoButton.onClick.AddListener(delegate { ToggleCharacterInfo(!charInfoState); });
    }

    private void Start()
    {
        for(int i = 0; i < itemSlot.Count; ++i)
        {
            switch (itemSlot[i].ItemCond)
            {
                case ItemType.HeadGear:
                    itemSlot[i].SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(31)));
                    break;
                case ItemType.ChestGear:
                    itemSlot[i].SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(32)));
                    break;
                case ItemType.WaistGear:
                    itemSlot[i].SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(34)));
                    break;
                case ItemType.FootGear:
                    itemSlot[i].SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(36)));
                    break;
                case ItemType.HandGear:
                    itemSlot[i].SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(33)));
                    break;
                case ItemType.ThighGear:
                    itemSlot[i].SetSlot(new ItemSlot(ItemInfoDataBase.FindItemInfo(35)));
                    break;
            }
        }
    }

    public void SetEquipTexture(Texture _texture)
    {
        equipImage.texture = _texture;
    }

    public void ToggleCharacterInfo(bool _state, bool _isImmediate = false)
    {
        charInfoState = _state;
        if (charInfoAnimation.IsActive())
        {
            charInfoAnimation.Kill();
        }
        charInfoAnimation = DOTween.Sequence();
        charInfoAnimation.
            Append(
                charInfo.DOAnchorPosX(_state ? charInfoDefaultPos.x : charInfoDefaultPos.x - charInfo.sizeDelta.x, _isImmediate ? 0 : charInfoAnimationTime)).
            OnKill(() => charInfoAnimation = null);
    }
}
