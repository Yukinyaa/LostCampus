using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;
    [Header("- Current UI")]
    [SerializeField] private UIComponent current;
    [Header("- Default UI")]
    [SerializeField] private UI_PopUp popUp;
    [SerializeField] private UI_Notice notice;
    [SerializeField] private UI_Effect effect;
    [Header("- UI Components")]
    [SerializeField] private UIComponent[] components;
    private Dictionary<Type, UIComponent> ui;
    public UIComponent Current { get => current; }

    private void Awake()
    {
        Instance = this;
        ui = new Dictionary<Type, UIComponent>();
        for (int i = 0; i < components.Length; ++i)
        {
            UIComponent target = components[i];
            target.Init();
            target.onFocus += () =>
            {
                target.transform.SetAsLastSibling();
                if ((object)current != null) current.Blur();
                current = target;
            };
            target.onBlur += () =>
            {
                target.transform.SetAsFirstSibling();
                current = null;
            };
            ui.Add(target.GetType(), target);
        }

        try
        {
            popUp.Init();
            popUp.transform.SetAsLastSibling();
        }
        catch (NullReferenceException)
        {
            popUp = GetComponentInChildren<UI_PopUp>();
            if (popUp != null)
            {
                popUp.Init();
                popUp.transform.SetAsLastSibling();
            }
        }

        try
        {
            notice.Init();
            notice.transform.SetAsLastSibling();
        }
        catch (NullReferenceException)
        {
            notice = GetComponentInChildren<UI_Notice>();
            if(notice != null)
            {
                notice.Init();
                notice.transform.SetAsLastSibling();
            }
        }

        try
        {
            effect.Init();
            effect.transform.SetAsLastSibling();
        }
        catch (NullReferenceException)
        {
            effect = GetComponentInChildren<UI_Effect>();
            if(effect != null)
            {
                effect.Init();
                effect.transform.SetAsLastSibling();
            }
        }
        
    }

    public void SetUIToTop(Transform _transform)
    {
        effect.SetUIToTop(_transform);
    }

    public T GetUI<T>() where T : UIComponent
    {
        try
        {
            return (T)ui[typeof(T)];
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// UI Component를 보이게 합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Show<T>() where T : UIComponent
    {
        try
        {
            UIComponent target = ui[typeof(T)];
            target.Show();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// UI Component를 보이게 하고, Focus 합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Focus<T>() where T : UIComponent
    {
        try
        {
            UIComponent target = ui[typeof(T)];
            target.Show();
            target.Focus();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// UI Component를 Blur 처리하고, Hide 합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Hide<T>() where T : UIComponent
    {
        try
        {
            UIComponent target = ui[typeof(T)];
            target.Blur();
            target.Hide();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// UI Component를 Blur 처리합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Blur<T>() where T : UIComponent
    {
        try
        {
            UIComponent target = ui[typeof(T)];
            target.Blur();
        }
        catch (Exception)
        {

        }
    }

    public void MakeNotice(string _content)
    {
        try
        {
            notice.AddNotice(_content);
        }
        catch (NullReferenceException)
        {

        }
    }

    public void PopUpExampleFunc()
    {
        MakePopUp_Selection().SetContent("개수 선택하는 팝업을 띄울래?\n무수한 선택지를 띄울래?").SetSelection("개수를 선택", "무수한 선택지").onClick += (index) =>
        {
            MakeNotice($"당신은 {index + 1} 번째 선택지를 골랐습니다.");
            if (index == 0)
            {
                MakePopUp_Counter().SetContent("골라봐! 범위는 0부터 9999까지!").SetValue(0, 5000, 9999).onClick += (current, max) =>
                {
                    MakeNotice($"당신은 {max}개 중에서 {current} 개를 골랐습니다.");
                };
            }
            else
            {
                MakePopUp_Selection().SetContent("무수한 선택지").SetSelection("하나", "둘", "셋", "넷", "다섯", "여섯", "일곱", "여덟").onClick += (index) =>
                {
                    MakeNotice($"당신은 {index + 1} 번째 선택지를 골랐습니다.");
                };
            }
        };
    }

    public void UIEventExampleFunc()
    {
        FadeOut(1).onEvent += () =>
        {
            Debug.Log("페이드 아웃 끝, 페이드 인 시작");
            FadeIn(1).onEvent += () => Debug.Log("페이드 인 끝");
        };
    }

    public UIEvent FadeIn(float _time)
    {
        try
        {
            return effect.FadeIn(_time);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public UIEvent FadeOut(float _time)
    {
        try
        {
            return effect.FadeOut(_time);
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public PopUp_Selection MakePopUp_Selection()
    {
        return popUp.MakePopUp_Selection();
    }

    public PopUp_Counter MakePopUp_Counter()
    {
        return popUp.MakePopUp_Counter();
    }

    public void ShowItemInfo(ItemSlot _data, Vector3 _pos)
    {
        popUp.ShowItemInfo(_data, _pos);
    }

    public void HideItemInfo()
    {
        popUp.HideItemInfo();
    }

    public PopUp_MiniMenu MakeMiniMenu(UIComponent _root)
    {
        return popUp.MakeMiniMenu(_root);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Mouse.current.position.ReadValue();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            for(int i = 0; i < results.Count; ++i)
            {
                if (results[i].gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
                {
                    UIComponent target = results[i].gameObject.GetComponentInParent<UIComponent>();
                    if ((object)target != null 
                        && !target.Equals(popUp)
                        && !target.Equals(notice)
                        && !target.Equals(effect)
                        && !target.Equals(current))
                    {
                        target.Focus();
                        break;
                    }
                }
            }
        }
    }
}
