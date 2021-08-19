using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    private static StarterInputAssets inputAssets = new StarterInputAssets();

    public static StarterInputAssets InputAssets => inputAssets;
    
    //사용예 :
    //InputManager.InputAssets.Player.Move.performed += x => Debug.Log( x.action.name+" performed");
}