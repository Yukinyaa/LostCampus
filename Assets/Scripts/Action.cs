using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Action : MonoBehaviour
{
    GameObject Door;

    void Start()
    {
        InputManager.FindAction("Action").performed += ToggleDoor;
    }

    void Update()
    {
        // if (_input.action)
        // {
        //     if(Door != null) {
        //         Door.GetComponent<Door>().ChangeDoorState();
        //         Door = null;
        //         _input.action = false;
        //     }
        // }
    }

    public void ToggleDoor(InputAction.CallbackContext context)
    {
        if (context.control.IsPressed())
        {
            if(Door != null) {
                Door.GetComponent<Door>().ChangeDoorState();
                Door = null;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Door")
        {
            Door = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == Door)
        {
            Door = null;
        }
    }
}
