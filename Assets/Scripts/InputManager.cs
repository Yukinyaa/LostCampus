using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    public InputActionAsset starterAssets;

    public InputActionAsset StarterAssets
    {
        get => starterAssets;
    }
}
