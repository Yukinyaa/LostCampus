using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_PlayerUI : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private TextMeshPro text_Name;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform);
    }

    public void SetName(string _content)
    {
        text_Name.SetText(_content);
    }
}
