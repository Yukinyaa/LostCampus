using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    GameObject Door;
    private StarterAssetsInputs _input;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.action)
        {
            if(Door != null) {
                Door.GetComponent<Door>().ChangeDoorState();
                Door = null;
                _input.action = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Door")
        {
            Door = other.gameObject;
        }
    }
}
