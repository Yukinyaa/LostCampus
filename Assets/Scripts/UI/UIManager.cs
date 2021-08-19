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
        MakePopUp_Selection().SetContent("���� �����ϴ� �˾��� ��﷡?\n������ �������� ��﷡?").SetSelection("������ ����", "������ ������").onClick += (index) =>
        {
            MakeNotice($"����� {index + 1} ��° �������� ������ϴ�.");
            if (index == 0)
            {
                MakePopUp_Counter().SetContent("����! ������ 0���� 9999����!").SetValue(5000, 9999).onClick += (current, max) =>
                {
                    MakeNotice($"����� {max}�� �߿��� {current} ���� ������ϴ�.");
                };
            }
            else
            {
                MakePopUp_Selection().SetContent("������ ������").SetSelection("�ϳ�", "��", "��", "��", "�ټ�", "����", "�ϰ�", "����").onClick += (index) =>
                {
                    MakeNotice($"����� {index + 1} ��° �������� ������ϴ�.");
                };
            }
        };
    }

    public void UIEventExampleFunc()
    {
        FadeOut(1).onEvent += () =>
        {
            Debug.Log("���̵� �ƿ� ��, ���̵� �� ����");
            FadeIn(1).onEvent += () => Debug.Log("���̵� �� ��");
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
