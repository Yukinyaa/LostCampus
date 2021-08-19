using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlayerInvSlot : UI_ItemSlot
{
    public override UI_ItemContainer GetRivalContainerUI()
    {
        return UIManager.Instance.GetUI<UI_Storage>();
    }
}
