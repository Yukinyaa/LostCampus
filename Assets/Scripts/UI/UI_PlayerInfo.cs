using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInfo : MonoBehaviour
{
    private Status PlayerStatus;
    public GameObject HP, AP;
    // Start is called before the first frame update
    void Awake()
    {
        this.PlayerStatus = GetComponentInParent<Status>();
    }

    // Update is called once per frame
    void Update()
    {
        this.HP.BroadcastMessage("Set", new float[2]{ this.PlayerStatus.Hp, this.PlayerStatus.MaxHp });
        this.AP.BroadcastMessage("Set", new float[2] { this.PlayerStatus.Ap, this.PlayerStatus.MaxAp });
    }
}
