using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIComponent : MonoBehaviour
{
    [Header("UI Component")]
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected CanvasGroup canvasGroup;

    public System.Action onFocus;
    public System.Action onBlur;

    public bool IsShow { get => canvasGroup.alpha >= 1; }
    public bool IsFocus { get => UIManager.Instance.Current.Equals(this); }
    public bool IsActive { get => canvasGroup.alpha >= 1 && canvasGroup.blocksRaycasts && canvasGroup.interactable; }

    public bool IsTooltip { get; private set; }
    public bool IsMiniMenu { get; private set; }
    public RectTransform RectTransform { get => rectTransform; }

    public virtual void Init()
    {
        if((object)rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if((object)canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        onFocus += OnFocus;
        onBlur += OnBlur;
    }

    public virtual void SetActive(bool _state, bool _callback = true)
    {
        try
        {
            canvasGroup.alpha = _state ? 1 : 0;
            canvasGroup.blocksRaycasts = _state;
            canvasGroup.interactable = _state;
            if (_callback)
            {
                if (_state) OnActive();
                else OnInactive();
            }
        }
        catch (NullReferenceException)
        {
            Debug.Log("CanvasGroup 컴포넌트를 추가해야 합니다.");
        }
    }

    public void Focus()
    {
        UIManager.Instance.Focus(this);
        onFocus?.Invoke();
    }

    protected virtual void OnActive()
    {

    }

    protected virtual void OnInactive()
    {

    }

    protected virtual void OnFocus()
    {

    }

    protected virtual void OnBlur()
    {
        IsTooltip = false;
        IsMiniMenu = false;
    }

    public virtual void On()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void Off()
    {
        gameObject.SetActive(false);
    }    
    
    public virtual void Toggle()
    {        
        if (isActiveAndEnabled)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    public void MakeTooltip(ItemSlot _data, Vector2 _pos)
    {
        if (!IsTooltip && !IsMiniMenu)
        {
            UIManager.Instance.ShowItemInfo(_data, _pos);
            IsTooltip = true;
        }
    }

    public void HideTooltip()
    {
        if (IsTooltip)
        {
            UIManager.Instance.HideItemInfo();
            IsTooltip = false;
        }
    }

    public PopUp_MiniMenu MakeMiniMenu()
    {
        IsMiniMenu = true;
        PopUp_MiniMenu miniMenu = UIManager.Instance.MakeMiniMenu(this);
        miniMenu.onClick += (index) => IsMiniMenu = false;
        return miniMenu;
    }

    public void Test() => Debug.Log("Test");
}
