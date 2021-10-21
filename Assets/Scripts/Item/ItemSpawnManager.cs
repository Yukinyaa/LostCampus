using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ItemSpawnManager : Singleton<ItemSpawnManager>
{
    [SerializeField]
    private GameObject baseItemPrefab;

    public GameObject BaseItemPrefab
    {
        get => baseItemPrefab;
        set => baseItemPrefab = value;
    }
}
