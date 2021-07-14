using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shelter : MonoBehaviour
{
    public Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }


    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.transform.name);
                switch (hit.transform.name)
                {
                    case "Armory":
                        UIManager.Instance.GetUI<UI_Armory>().SetActive(true);
                        break;
                    case "Chest":
                    case "CraftingTable":
                        UIManager.Instance.GetUI<UI_CraftingTable>().SetActive(true);
                        break;
                }
            }
        }
    }
}
