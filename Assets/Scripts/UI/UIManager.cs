using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;
    [Header("UI Manager")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform rectTransform;
    [Header("- Current UI")]
    [SerializeField] private UIComponent current;
    [Header("- Default UI")]
    [SerializeField] private UI_PopUp popUp;
    [SerializeField] private UI_Notice notice;
    [SerializeField] private UI_Effect effect;
    [Header("- UI Components")]
    [SerializeField] private UIComponent[] components;
    private Dictionary<Type, UIComponent> ui;
    public Canvas Canvas { get => canvas; }
    public RectTransform RectTransform { get => rectTransform; }
    public UIComponent Current { get => current; }
    public UI_PopUp PopUp { get => popUp; }

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
                current = null;
            };
            ui.Add(target.GetType(), target);
        }

        try
        {
            popUp.transform.SetAsLastSibling();
        }
        catch (NullReferenceException)
        {
            popUp = GetComponentInChildren<UI_PopUp>();
            if (popUp != null)
            {
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
    /// UI Component�� ���̰� �մϴ�.
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
    /// UI Component�� ���̰� �ϰ�, Focus �մϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Focus<T>() where T : UIComponent
    {
        try
        {
            UIComponent target = ui[typeof(T)];
            target.Focus();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// UI Component�� Blur ó���մϴ�.
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

    /// <summary>
    /// UI Component�� Blur ó���ϰ�, Hide �մϴ�.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Hide<T>() where T : UIComponent
    {
        try
        {
            UIComponent target = ui[typeof(T)];
            target.Hide();
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
        MakeSelection().SetContent("���� �����ϴ� �˾��� ��﷡?\n������ �������� ��﷡?").SetSelection("������ ����", "������ ������").onClick += (index) =>
        {
            MakeNotice($"����� {index + 1} ��° �������� ������ϴ�.");
            if (index == 0)
            {
                MakeCounter().SetContent("����! ������ 0���� 9999����!").SetValue(0, 5000, 9999).onClick += (current, max) =>
                {
                    MakeNotice($"����� {max}�� �߿��� {current} ���� ������ϴ�.");
                };
            }
            else
            {
                MakeSelection().SetContent("������ ������").SetSelection("�ϳ�", "��", "��", "��", "�ټ�", "����", "�ϰ�", "����").onClick += (index) =>
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

    public PopUp_Selection MakeSelection()
    {
        return popUp.MakeSelection();
    }

    public PopUp_Counter MakeCounter()
    {
        return popUp.MakeCounter();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Mouse.current.position.ReadValue();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            if(results.Count > 0)
            {
                if (results[0].gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
                {
                    UIComponent target = results[0].gameObject.GetComponentInParent<UIComponent>();
                    if ((object)target != null 
                        && !target.Equals(popUp)
                        && !target.Equals(notice)
                        && !target.Equals(effect)
                        && !target.Equals(current))
                    {
                        target.Focus();
                    }
                }
            }
        }
    }
}
