using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class UI_NItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected static bool isDrag = false;               // �巡�� ������ �Ǻ��ϴ� ����
    protected static bool isDrop = false;               // ����� �ؾ� �ϴ��� �Ǻ��ϴ� ���� (true�� ��� ��� ������ ����� ���� �ʾҴٴ� ��)
    protected static bool isSwap = false;               // ��ȯ�� �Ǿ�� �ϴ��� �Ǻ��ϴ� ���� (true�� ��� ��ȯ�� �Ǿ�� �Ѵٴ� ��)
    protected static UI_NItemSlot currentSlot = null;
    protected static Sequence toolTipSequence = null;

    [SerializeField] protected Image image_Content;
    [SerializeField] protected TextMeshProUGUI text_Count;
    [SerializeField] protected Transform item;
    [SerializeField] protected ItemType itemCond;
    public ItemType ItemCond => itemCond;

    protected ItemSlot slotData;
    public ItemSlot SlotData
    {
        get => slotData;
        set => slotData = value;
    }

    protected UI_NItemContainer container;
    public UI_NItemContainer Container
    {
        get => container;
        set => container = value;
    }

    private int slotIndex = -1;
    public int SlotIndex
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    public UI_NItemSlot SetSlot(ItemSlot _slotData)
    {
        if (_slotData == null)
        {
            if (SlotData != null)
                SlotData.OnValueChanged -= OnValueChanged;
            SlotData = null;
            image_Content.sprite = null;
            text_Count.text = string.Empty;
        }
        else
        {
            if (SlotData != null)
                SlotData.OnValueChanged -= OnValueChanged;

            SlotData = (ItemSlot)_slotData;
            image_Content.sprite = ItemInfoDataBase.GetSprite(_slotData.Sprite);
            SlotData.OnValueChanged += OnValueChanged;
            text_Count.text = SlotData.Amount.ToString();
        }
        return this;
    }
    public UI_NItemSlot SetIndex(int _index)
    {
        SlotIndex = _index;
        return this;
    }

    public UI_NItemSlot SetContainer(UI_NItemContainer _container)
    {
        Container = _container;
        return this;
    }

    public UI_NItemSlot SetParent(Transform _parent)
    {
        transform.SetParent(_parent);
        return this;
    }


    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (SlotData != null)
        {
            isDrag = true;
            isDrop = true;
            isSwap = true;
            currentSlot = this;
            UIManager.Instance.SetUIToTop(item);
            Container.HideTooltip();
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isDrag && SlotData != null)
        {
            item.position = eventData.position;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (isDrag)
        {
            if (currentSlot != null)
            {
                if (itemCond.HasFlag(currentSlot.SlotData.Type))
                {
                    ItemSlot tempData = SlotData;
                    SlotData = currentSlot.SlotData;
                    if (tempData != null)
                    {
                        currentSlot.SlotData = tempData;
                        this.SetSlot(SlotData);
                    }
                    else isSwap = false;
                }
                else
                {
                    isSwap = false;
                    UIManager.Instance.MakeNotice("�������� �ű� �� �����ϴ�.");
                }
                isDrop = false;
            }
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            if (isSwap)
            {
                SlotData = currentSlot.SlotData;
                this.SetSlot(SlotData);
            }
            if (isDrop)
            {
                Debug.Log("�ƹ��������� ��� �̺�Ʈ�� �߻����� ����");
            }
            StopDrag(currentSlot.Container);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (SlotData == null || isDrag) return;
        if (toolTipSequence.IsActive())
        {
            toolTipSequence.Kill();
        }
        toolTipSequence = DOTween.Sequence();
        toolTipSequence.
            AppendInterval(0.5f).
            AppendCallback(() => Container.ShowTooltip(this.SlotData, eventData.position)).
            OnKill(() => toolTipSequence = null);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (SlotData == null) return;
        if (toolTipSequence.IsActive())
        {
            toolTipSequence.Kill();
        }
        Container.HideTooltip();
    }

    public static void StopDrag(UIComponent _container)
    {
        if (isDrag && currentSlot.Container == _container)
        {
            currentSlot.item.SetParent(currentSlot.transform);
            currentSlot.item.localPosition = Vector3.zero;
            currentSlot = null;
            isDrag = false;
            isDrop = false;
            isSwap = false;
        }
    }

    public void OnValueChanged(ItemSlot _slotData)
    {
        if (_slotData.Amount <= 0) SetSlot(null);
        else text_Count.text = SlotData.Amount.ToString();
    }


}
