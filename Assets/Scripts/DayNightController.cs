using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayNightController : NetworkBehaviour
{
    [SerializeField] Light sun;
    [SerializeField] Light moon;

    [SerializeField] [Range(0, 24)]
    [SyncVar] float timeOfDay; // in hours

    [SerializeField] float minutePerDay = 5;
    [SerializeField] Volume skyVolume;


    public float TimeOfDay { get => timeOfDay; }

    bool IsDay{ get { if (6 < timeOfDay && timeOfDay < 18) return true; else return false; } }

    void Update()
    {
        timeOfDay += Time.deltaTime / 60f / minutePerDay * 24f;
        timeOfDay %= 24;
        UpdateSunMoonToTime();
    }

    private void OnValidate()
    {
        UpdateSunMoonToTime();
    }

    void UpdateSunMoonToTime()
    {
        float sunRotation = Mathf.Lerp(-90, 270, timeOfDay / 24f);
        float moonRotation = sunRotation - 180;

        sun.transform.rotation = Quaternion.Euler(sunRotation, -180f, 20);
        moon.transform.rotation = Quaternion.Euler(moonRotation, -180f, 20);
        
        
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
