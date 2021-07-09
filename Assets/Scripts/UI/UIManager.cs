using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;

    [SerializeField]
    private UIComponent[] components;
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


}
