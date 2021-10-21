using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private InputActionAsset inputAssets;
    private InputActionMap actionMap;

    public InputActionAsset InputAssets => inputAssets;

    public InputActionMap ActionMap
    {
        get
        {
            if ((object) actionMap == null)
            {
                actionMap = inputAssets.FindActionMap("Player");
            }

            return actionMap;
        }
    }
    
    public static InputAction FindAction(string name)
    {
        return Instance.ActionMap.FindAction(name);
    }
    
    //사용예 :
    //InputManager.Instance.ActionMap.FindAction("ToggleInventory").performed += x => Debug.Log( x.action.name+" performed");
    //눌렸을때를 확인하기 위해서는 x.controls.isPressed을 사용
}