using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIComponent : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;
    public virtual void Init()
    {

    }

    public virtual void SetActive(bool _state)
    {
        try
        {
            canvasGroup.alpha = _state ? 1 : 0;
            canvasGroup.blocksRaycasts = _state;
            canvasGroup.interactable = _state;
        }
        catch (NullReferenceException)
        {
            Debug.Log("CanvasGroup ������Ʈ�� �߰��ؾ� �մϴ�.");
        }
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
}
