using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Effect : UIComponent
{
    [SerializeField] private CanvasGroup fadeObject;
    private Sequence fadeSequence;

    public UIEvent FadeIn(float _time)
    {
        UIEvent newEvent = new UIEvent();
        if (fadeSequence.IsActive())
        {
            fadeSequence.Kill(true);
        }

        fadeObject.alpha = 1;
        fadeObject.interactable = false;
        fadeObject.blocksRaycasts = false;
        fadeSequence = DOTween.Sequence();
        fadeSequence.
            Append(fadeObject.DOFade(0, _time)).
            OnComplete(() => newEvent.Invoke()).
            OnKill(() => fadeSequence = null);

        return newEvent;
    }

    public UIEvent FadeOut(float _time)
    {
        UIEvent newEvent = new UIEvent();
        if (fadeSequence.IsActive())
        {
            fadeSequence.Kill(true);
        }

        fadeObject.alpha = 0;
        fadeObject.interactable = true;
        fadeObject.blocksRaycasts = true;
        fadeSequence = DOTween.Sequence();
        fadeSequence.
            Append(fadeObject.DOFade(1, _time)).
            OnComplete(() => newEvent.Invoke()).
            OnKill(() => fadeSequence = null);

        return newEvent;
    }

    public void SetDarkScreen(bool _state)
    {
        if (!fadeSequence.IsActive())
        {
            fadeObject.alpha = _state ? 1 : 0;
            fadeObject.interactable = _state;
            fadeObject.blocksRaycasts = _state;
        }
    }

}
