using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_Notice : UIComponent
{
    private static int MAX_LINECOUNT = 8;
    private static int MAX_NOTICECOUNT = 5;

    private Queue<string> noticeQueue;
    private Queue<Sequence> sequenceQueue;
    private Queue<UI_NoticeLine> storageQueue;

    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform storage;
    [Header("- Prefab")]
    [SerializeField] private UI_NoticeLine noticeLine;


    public override void Init()
    {
        noticeQueue = new Queue<string>();
        storageQueue = new Queue<UI_NoticeLine>(MAX_LINECOUNT);
        sequenceQueue = new Queue<Sequence>(MAX_NOTICECOUNT);
        float lineHeight = rectTransform.rect.height / MAX_NOTICECOUNT;
        for (int lineIndex = 0; lineIndex < MAX_LINECOUNT; ++lineIndex)
        {
            storageQueue.Enqueue(
                Instantiate(noticeLine).
                SetParent(storage).
                SetContent(string.Empty).
                SetLineHeight(lineHeight));
        }
    }

    public void AddNotice(string _content)
    {
        noticeQueue.Enqueue(_content);
        if(sequenceQueue.Count < MAX_NOTICECOUNT)
        {
            MakeNotice();
        }
    }

    private void MakeNotice()
    {
        Sequence newSequence = DOTween.Sequence();
        UI_NoticeLine newNotice = storageQueue.Dequeue().SetParent(content).SetContent(noticeQueue.Dequeue());
        newNotice.CanvasGroup.alpha = 0f;
        newSequence.
            Append(newNotice.CanvasGroup.DOFade(1, 0.5f)).
            AppendInterval(2f).
            Append(newNotice.CanvasGroup.DOFade(0, 0.5f)).
            AppendCallback(() => { storageQueue.Enqueue(newNotice.SetParent(storage).SetContent(string.Empty)); }).
            AppendInterval(1f).
            OnComplete(() =>
            {
                sequenceQueue.Dequeue();
                if(noticeQueue.Count > 0)
                {
                    MakeNotice();
                }
            });
        sequenceQueue.Enqueue(newSequence);
    }
}
