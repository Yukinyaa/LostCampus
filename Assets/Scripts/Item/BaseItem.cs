using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BaseItem : Entity
{

    #region info
    [NonSerialized] private ItemInfo _itemInfo;
    public ItemInfo itemInfo
    {
        get
        {
            if (_itemInfo == null)
            {
                _itemInfo = ItemInfoDataBase.FindItemInfo(infoID);
            }
            return _itemInfo;
        }
    }

    [SerializeField][SyncVar]
    private int amount = 1;
    #endregion
    
    [SerializeField] 
    private int infoID = 0;
    [SerializeField]
    private SpriteRenderer spriteRend;
    [Header("회전속도")][SerializeField]
    private float rotSpeed = 1f;
    [Header("픽업까지의 유예시간")] [SerializeField]
    private float pickUpCoolTime = 0.5f;
    
    private float pickUpTimer = 0f;

    public override void OnStartClient()
    {
        _itemInfo = ItemInfoDataBase.FindItemInfo(infoID);
        spriteRend.sprite = ItemInfoDataBase.GetSprite(_itemInfo.sprite);
    }

    public BaseItem() {}
    
    public BaseItem(ItemInfo itemInfo, int _amount = 1, string _instanceId = null)
    {
        _itemInfo = itemInfo;
        amount = _amount;
        instanceID = _instanceId;
    }
    
    public BaseItem(int itemId, int _amount = 1, string _instanceId = null)
    {
        infoID = itemId;
        amount = _amount;
        instanceID = _instanceId;
    }

    public void init(int itemId, int _amount = 1, string _instanceId = null)
    {
        infoID = itemId;
        amount = _amount;
        instanceID = _instanceId;
    }

    void OnEnable()
    {
        pickUpTimer = 0f;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up*rotSpeed);
        pickUpTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (pickUpTimer >= pickUpCoolTime)
        {
            try
            {
                PlayerInventory inv = other.GetComponentInParent<PlayerInventory>();
                int remain = 0;
                if (inv != null)
                {
                    remain = inv.TryUpdateItemById(_itemInfo.id,amount);
                }
                CmdChangeAmount(-(amount - remain));

            }
            catch (NullReferenceException)
            {
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdChangeAmount(int i)
    {
        amount += i;
        if (amount <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}