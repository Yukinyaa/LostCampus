using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Status PlayerStatus;
    private MeshCollider WeaponCollider;
    private GameObject PlayerGameObject;
    private List<Status> OtherStatus;

    private void Awake()
    {
        Debug.Assert(PlayerStatus != null);

        this.WeaponCollider = GetComponentInChildren<MeshCollider>();
        this.WeaponCollider.enabled = false;
        this.PlayerStatus = GetComponentInParent<Status>();
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
            if (other.GetComponentInParent<Status>() != PlayerStatus)
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

                        OtherStatus.HP = OtherStatus.HP - this.PlayerStatus.ATK;

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
