using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Equip : UIComponent
{
    [Header("- Equip")]
    [SerializeField] private RawImage equipImage;
    [SerializeField] private GameObject[] equipSlot;

    [Header("- Character Info")]
    [SerializeField] private RectTransform charInfo;
    [SerializeField] private TextMeshProUGUI charInfoText;
    [SerializeField] private Button charInfoButton;
    [SerializeField] private Image charInfoButtonImage;
    [SerializeField] private float charInfoAnimationTime;

    
    private bool charInfoState;
    private Sequence charInfoAnimation;
    private Vector2 charInfoDefaultPos;

    public override void Init()
    {
        charInfoState = false;
        charInfoDefaultPos = charInfo.anchoredPosition;
        charInfo.anchoredPosition = new Vector2(charInfoDefaultPos.x - charInfo.sizeDelta.x, charInfoDefaultPos.y);
        charInfoButton.onClick.AddListener(delegate { ToggleCharacterInfo(!charInfoState); });
    }

    public void SetEquipTexture(Texture _texture)
    {
        equipImage.texture = _texture;
    }

    public void ToggleCharacterInfo(bool _state)
    {
        charInfoState = _state;
        if (charInfoAnimation.IsActive())
        {
            charInfoAnimation.Kill();
        }
        charInfoAnimation = DOTween.Sequence();
        charInfoAnimation.
            Append(
                charInfo.DOAnchorPosX(_state ? charInfoDefaultPos.x : charInfoDefaultPos.x - charInfo.sizeDelta.x, charInfoAnimationTime)).
            OnKill(() => charInfoAnimation = null);

    }
}
