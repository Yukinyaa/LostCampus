using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class KillOnTouch : NetworkBehaviour
{
    GameObject parent;
    [Command]
    void CmdKillOther(GameObject gameObject)
    { 
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Player"))
        {
            if(collision.gameObject != parent)
                CmdKillOther(collision.gameObject);
        }
    }
}
