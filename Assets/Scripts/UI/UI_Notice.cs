using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Notice : UIComponent
{
    private static int MAX_LINECOUNT = 8;

    private Queue<string> noticeQueue;
    private Queue<UI_NoticeLine> storageQueue;

    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform storage;
    [Header("- Prefab")]
    [SerializeField] private UI_NoticeLine noticeLine;


    public override void Init()
    {
        noticeQueue = new Queue<string>();
        storageQueue = new Queue<UI_NoticeLine>(MAX_LINECOUNT);
        for (int lineIndex = 0; lineIndex < MAX_LINECOUNT; ++lineIndex)
        {
            storageQueue.Enqueue(
                Instantiate(noticeLine).
                SetParent(storage).
                SetContent(string.Empty));
        }
    }

    public void AddNotice(string _content)
    {
        noticeQueue.Enqueue(_content);

    }
}
