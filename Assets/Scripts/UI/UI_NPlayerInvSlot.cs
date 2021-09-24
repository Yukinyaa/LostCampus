using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UI_NPlayerInvSlot : UI_NItemSlot
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (SlotData == null) return;
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                Container.HideTooltip();
                if (Keyboard.current[Key.LeftShift].IsPressed())
                {

                }
                else
                {
                    Container.MakeMiniMenu().SetPosition(eventData.position).SetSelection("사용", "이동", "테스트").onClick += (index) =>
                    {
                        switch (index)
                        {
                            case 0:
                                Debug.Log("사용");
                                break;
                            case 1:
                                Debug.Log("이동");
                                break;
                            case 2:
                                Debug.Log("테스트");
                                break;
                        }
                    };
                }
                break;
        }
    }
}
