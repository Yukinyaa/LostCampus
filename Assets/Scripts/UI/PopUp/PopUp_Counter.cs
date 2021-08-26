using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp_Counter : MonoBehaviour
{
    private static Vector2 DEFAULT_SIZE = new Vector2(600, 300);

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popUp;
    [SerializeField] private TextMeshProUGUI text_Content;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text_Counter;

    // 현재 값과 최대 값을 반환, Accept를 눌렀을 때만 발동
    public System.Action<int, int> onClick;

    public void Init()
    {
        popUp.sizeDelta = DEFAULT_SIZE;
        slider.onValueChanged.AddListener(delegate { SetCounter((int)slider.value); });
    }

    public PopUp_Counter SetContent(string _content)
    {
        text_Content.SetText(_content);
        return this;
    }

    private PopUp_Counter SetCounter(int _num)
    {
        text_Counter.SetText($"{_num}");
        return this;
    }

    public PopUp_Counter SetCurrentValue(int _currentValue)
    {
        slider.value = Mathf.Min(slider.maxValue, _currentValue);
        return this;
    }

    public PopUp_Counter SetMaxValue(int _maxValue)
    {
        slider.maxValue = _maxValue;
        return this;
    }

    public PopUp_Counter SetValue(int _currentValue, int _maxValue)
    {
        return SetMaxValue(_maxValue).SetCurrentValue(_currentValue);
    }

    private PopUp_Counter SetActive(bool _state)
    {
        canvasGroup.alpha = _state ? 1 : 0;
        canvasGroup.interactable = _state;
        canvasGroup.blocksRaycasts = _state;
        return this;
    }

    public PopUp_Counter Clear()
    {
        onClick = null;
        return SetContent(string.Empty).SetCounter(0).SetActive(true);
    }

    public void OnClick_Accept()
    {
        SetActive(false);
        if(onClick != null)
        {
            System.Action<int, int> popupEvent = new System.Action<int, int>(onClick);
            popupEvent?.Invoke((int)slider.value, (int)slider.maxValue);
        }
    }

    public void OnClick_Cancel()
    {
        SetActive(false);
    }

    public void OnClick_AddValue(int _value)
    {
        slider.value += _value;
        if (slider.value < 0) slider.value = 0;
        if (slider.value >= slider.maxValue) slider.value = slider.maxValue;
    }
}
