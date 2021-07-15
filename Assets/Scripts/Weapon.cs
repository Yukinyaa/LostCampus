using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Status Status;
    private void Awake()
    {
        this.Status=gameObject.AddComponent<Status>();
        this.Status.HP = 1000;
        this.Status.ATK = 1;
    }
}
