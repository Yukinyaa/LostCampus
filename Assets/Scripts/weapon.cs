using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private MeshCollider WeaponCollider;
    private GameObject PlayerGameObject;
    private ThirdPersonController PlayerController;
    private Status PlayerStatus;
    private List<Status> OtherStatus;

    private void Awake()
    {
        this.WeaponCollider = GetComponentInChildren<MeshCollider>();
        this.WeaponCollider.enabled = false;
        this.PlayerController = GetComponentInParent<ThirdPersonController>();
        this.PlayerGameObject = PlayerController.gameObject;
        this.PlayerStatus = PlayerGameObject.GetComponent<Status>();
        this.PlayerController.Weapon = this;
    }

    public void Set(bool tmp)
    {
        if (tmp)
        {
            WeaponCollider.enabled = true;
            OtherStatus = new List<Status>();
        }
        else{
            WeaponCollider.enabled = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.GetComponentInParent<ThirdPersonController>() != PlayerController)
            {
                Status OtherStatus;
                try
                {
                    if (!other.gameObject.TryGetComponent<Status>(out OtherStatus))
                        OtherStatus = other.gameObject.GetComponentInParent<Status>();
                    if (!this.OtherStatus.Contains(OtherStatus))
                    {
                        this.OtherStatus.Add(OtherStatus);
                        //여기서부터 데미지 처리

                        OtherStatus.HP = OtherStatus.HP-this.PlayerStatus.ATK;

                        Debug.Log(OtherStatus.HP);
                    }
                }
                catch (NullReferenceException)
                {
                }
            }
        }
        catch (NullReferenceException)
        {
        }

    }
}
