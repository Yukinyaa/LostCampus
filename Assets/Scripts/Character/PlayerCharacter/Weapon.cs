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

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Skeleton")
        {
            try
            {
                Status otherStatus = other.GetComponentInParent<Status>();
                if (otherStatus.MyFaction != this.playerStatus.MyFaction)
                {
                    if (!this.otherStatus.Contains(otherStatus))
                    {
                        this.otherStatus.Add(otherStatus);
                        //여기서부터 데미지 처리
                        otherStatus.GetAttacked(playerStatus.Atk, other.ClosestPointOnBounds(transform.position));

                        Debug.Log(otherStatus.name + ":" + otherStatus.Hp);
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }
        

    }
}
