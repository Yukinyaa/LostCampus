using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class Weapon : MonoBehaviour
{
    [FormerlySerializedAs("playerStatus")] 
    [SerializeField] private Status myStatus;
    private MeshCollider WeaponCollider;
    private GameObject myGameObject;
    private List<Status> otherStatus;

    private void Awake()
    {
        Debug.Assert(myStatus != null);

        this.WeaponCollider = GetComponentInChildren<MeshCollider>();
        this.WeaponCollider.enabled = false;
        this.myStatus = GetComponentInParent<Status>();
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
                if (otherStatus.MyFaction != this.myStatus.MyFaction)
                {
                    if (!this.otherStatus.Contains(otherStatus))
                    {
                        this.otherStatus.Add(otherStatus);
                        //여기서부터 데미지 처리
                        playerStatus.PlayHitParticle(transform.position);
                        otherStatus.GetAttacked(myStatus.Atk, other.ClosestPointOnBounds(transform.position));

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
