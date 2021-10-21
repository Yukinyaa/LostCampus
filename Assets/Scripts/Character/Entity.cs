using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    protected string instanceID;

    public string InstanceID
    {
        get => instanceID;
        set => instanceID = value;
    }
}
