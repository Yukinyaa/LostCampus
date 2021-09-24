using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PopUp : UIComponent
{
    [Header("- PopUp")]
    [SerializeField] PopUp_Counter popUp_Counter;
    [SerializeField] PopUp_Selection popUp_Selection;

    public override void Init()
    {
        popUp_Counter.Init();
        popUp_Selection.Init();
    }

    public PopUp_Counter MakePopUp_Counter()
    {
        return popUp_Counter.Clear();
    }

    public PopUp_Selection MakePopUp_Selection()
    {
        return popUp_Selection.Clear();
    }

}
