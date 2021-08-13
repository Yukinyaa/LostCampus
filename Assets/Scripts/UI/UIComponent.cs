using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIComponent : MonoBehaviour
{
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] protected CanvasGroup canvasGroup;

    public bool IsShow { get => canvasGroup.alpha >= 1; }
    public bool IsActive { get => canvasGroup.alpha >= 1 && canvasGroup.blocksRaycasts && canvasGroup.interactable; }
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
            Debug.Log("CanvasGroup 컴포넌트를 추가해야 합니다.");
        }
    }

}
