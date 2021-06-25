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

    [SerializeField] Volume skyVolume;
    
    PhysicallyBasedSky phsicalSky;

    private void Start()
    {
        skyVolume.profile.TryGet(out phsicalSky);
    }
     
    private void OnValidate()
    {
        skyVolume.profile.TryGet(out phsicalSky);
        UpdateSkyToTime();
    }

    private void Update()
    {
        UpdateSkyToTime();
    }

    void UpdateSkyToTime()
    {
        float skyRotation = GetSkyRotation();

        // this should have sun/moon as child
        this.transform.rotation = Quaternion.Euler(skyRotation, 0, 0);

        phsicalSky.spaceRotation.value = moon.transform.rotation.eulerAngles;
        


        if (TimeController.Instance.IsDay)
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

    private float GetSkyRotation()
    {
        return Mathf.Lerp(-90, 270, TimeController.Instance.TimeOfDay / 24f);
    }
}
