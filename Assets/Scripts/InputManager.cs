using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    private Input.StarterAssets starterAssets;

    public Input.StarterAssets StarterAssets
    {
        get => starterAssets;
    }
}
