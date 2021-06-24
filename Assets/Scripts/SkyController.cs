using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SkyController : NetworkBehaviour
{
    [SerializeField] Light sun;
    [SerializeField] Light moon;

    [SerializeField] [Range(0, 24)]
    [SyncVar] float timeOfDay; // in hours

    [SerializeField] float minutePerDay = 5;
    [SerializeField] Volume skyVolume;

    [SerializeField]


    public float TimeOfDay { get => timeOfDay; }

    bool IsDay{ get { if (6 < timeOfDay && timeOfDay < 18) return true; else return false; } }
    

    private void Update()
    {
        if (isServer)
        {
            ServerUpdate();
        }
        UpdateSunMoonToTime();
    }

    //[Server]
    void ServerUpdate()
    {
        timeOfDay += Time.deltaTime / 60f / minutePerDay * 24f;
        timeOfDay %= 24;
    }

    private void OnValidate()
    {
        UpdateSunMoonToTime();
    }

    void UpdateSunMoonToTime()
    {
        float skyRotation = Mathf.Lerp(-90, 270, timeOfDay / 24f);

        this.transform.rotation = Quaternion.Euler(skyRotation, 0, 0);
        skyVolume.profile.TryGet(out PhysicallyBasedSky sky);
        if (sky != null)
        {
            sky.spaceRotation.value = moon.transform.rotation.eulerAngles;
        }
        


        if (IsDay)
        {
            sun.enabled = true;
            moon.enabled = false;
        }
        else
        {
            sun.enabled = false;
            moon.enabled = true;
        }
    }
}
