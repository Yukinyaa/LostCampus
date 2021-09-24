using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInventory : ItemContainer
{
    private string playerID = "";

    #region NetworkEvent
    public override void cacheUI()
    {
        if (isLocalPlayer)
        {
            InputManager.Instance.ActionMap.FindAction("ToggleInventory").performed +=
                OnInventoryPressed;
            InventoryUI = UIManager.Instance.GetUI<UI_Inventory>();
        }
    }
    #endregion

    public override void InvokeSaveInventoryData()
    {
        checkPlayerID();
        SaveInventoryData( playerID +"_PlayerInventory.txt", SaveLoadHelper.SERVER_SAVE_PATH);
    }
    
    public override ItemSlot[] InvokeLoadInventoryData()
    {
        checkPlayerID();
        return LoadInventoryData(playerID +"_PlayerInventory.txt", SaveLoadHelper.SERVER_SAVE_PATH);
    }

    public void checkPlayerID()
    {
        if (playerID.Equals(""))
        {
            if(SteamManager.Initialized)
                playerID = SteamUser.GetSteamID().ToString();
            else
            {
                SaveLoadHelper.LocalLoad("PlayerID.ini", SaveLoadHelper.Client_SAVE_PATH,out playerID);
                if (playerID == null)
                {
                    playerID = Guid.NewGuid().ToString();
                    SaveLoadHelper.LocalSave(playerID, "PlayerID.ini", SaveLoadHelper.Client_SAVE_PATH);
                }
            }
        }
    }
    
    

    public void OnInventoryPressed(InputAction.CallbackContext context)
    {
        if(context.control.IsPressed())
            InventoryUI?.Toggle();
    }
    
}
