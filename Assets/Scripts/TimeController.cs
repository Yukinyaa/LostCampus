using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

class TimeController : NetworkBehaviour
{
    static TimeController instance;
    static public TimeController Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TimeController>();

            return instance;
        }
    }


    [SerializeField]
    [Range(0, 24)]
    [SyncVar]
    float timeOfDay; // in hour

    [SerializeField] float realtimeMinutePerDay = 5;


    public float TimeOfDay { get => timeOfDay; }

    public bool IsDay
    {
        get { 
            if (6 < timeOfDay && timeOfDay < 18)
                return true; 
            else
                return false;
        } 
    }


    private void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
    }


    //[Server]
    void ServerUpdate()
    {
        timeOfDay += Time.deltaTime / 60f / realtimeMinutePerDay * 24f;
        timeOfDay %= 24;
    }

}
