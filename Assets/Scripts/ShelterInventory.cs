using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ShelterInventory : ItemContainer
{
    public override void cacheUI()
    {
        InventoryUI = UIManager.Instance.GetUI<UI_Storage>();
    }

    public override void InvokeSaveInventoryData()
    {
        SaveInventoryData("shelter.txt", SaveLoadHelper.SERVER_SAVE_PATH);
    }

    public override ItemSlot[] InvokeLoadInventoryData()
    {
        return LoadInventoryData("shelter.txt", SaveLoadHelper.SERVER_SAVE_PATH);
    }
    

}
