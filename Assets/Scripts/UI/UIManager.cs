using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;
    [SerializeField] private UI_Notice notice;
    [SerializeField] private UI_Effect effect;
    [SerializeField] private UI_PopUp popUp;
    [Header("- UI Components")]
    [SerializeField] private UIComponent[] components;
    private Dictionary<Type, UIComponent> ui;

    private void Awake()
    {
        Instance = this;
        ui = new Dictionary<Type, UIComponent>();
        for (int i = 0; i < components.Length; ++i)
        {
            components[i].Init();
            ui.Add(components[i].GetType(), components[i]);
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
                MakePopUp_Counter().SetContent("골라봐! 범위는 0부터 9999까지!").SetValue(5000, 9999).onClick += (current, max) =>
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

}
