using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Status playerStatus;
    private MeshCollider WeaponCollider;
    private GameObject PlayerGameObject;
    private List<Status> otherStatus;

    private void Awake()
    {
        Debug.Assert(playerStatus != null);

        this.WeaponCollider = GetComponentInChildren<MeshCollider>();
        this.WeaponCollider.enabled = false;
        this.playerStatus = GetComponentInParent<Status>();
        this.otherStatus = new List<Status>();
    }

    public void Set(bool tmp)
    {
        if (tmp)
        {
            this.otherStatus.Clear();
            WeaponCollider.enabled = true;
        }
        else
        {
            WeaponCollider.enabled = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Skeleton")
        {
            try
            {
                Status otherStatus = other.GetComponentInParent<Status>();
                if (otherStatus.team != this.playerStatus.team)
                {
                    if (!this.otherStatus.Contains(otherStatus))
                    {
                        this.otherStatus.Add(otherStatus);
                        //여기서부터 데미지 처리

                        otherStatus.HP -= this.playerStatus.ATK;

                        Debug.Log(otherStatus.name + ":" + otherStatus.HP);
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }
        

    }
}
