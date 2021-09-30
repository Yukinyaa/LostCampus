
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Enemy : NonPlayableCharacter
{
    private string baseID;
    private Status status;
    private BaseAI baseAI;

    public string BaseID
    {
        get => baseID;
        set => baseID = value;
    }

    public Status Status
    {
        get => status;
        set => status = value;
    }

    public BaseAI BaseAI
    {
        get => baseAI;
        set => baseAI = value;
    }

    public void Awake()
    {
        status = GetComponent<Status>();
        baseAI = GetComponent<BaseAI>();
        status.OnDieEvent.AddListener(OnDie);
    }

    public void OnDie(GameObject obj)
    {
        DropTable droptable = DropTableDataBase.FindDropTable(BaseID);
        foreach (var pair in droptable.dropDict)
        {
            float result = Random.Range(0f, 1.0f);
            if(result <= pair.Value)
                CmdMakeItem(pair.Key);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdMakeItem(string id)
    {
        GameObject item = Instantiate<GameObject>(ItemSpawnManager.Instance.BaseItemPrefab, transform.position, quaternion.identity);
        item.GetComponent<BaseItem>().init(int.Parse(id));
        NetworkServer.Spawn(item);
    }
}
