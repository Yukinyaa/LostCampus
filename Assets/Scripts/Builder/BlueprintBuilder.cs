using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BlueprintBuilder : MonoBehaviour
{
    [Header("- Id는 자동으로 작성됩니다.")]
    public Blueprint[] blueprints;

    private void Awake()
    {
        Destroy(gameObject);
    }

    public void BuildData()
    {
        for (int i = 0; i < blueprints.Length; ++i)
        {
            blueprints[i].id = i;
        }
        Debug.Log(JsonHelper.ToJson(blueprints, true));
        SaveBlueprintData();
    }

    public void SaveBlueprintData()
    {
        try
        {
            string json = JsonHelper.ToJson(blueprints, true);
            string path = Application.persistentDataPath + "/Server/" + "blueprint.txt";
            File.WriteAllText(path, json);
        }
        catch (Exception)
        {

        }
    }

    public Blueprint[] LoadBlueprintData()
    {
        string path = Application.persistentDataPath + "/Server/" + "blueprint.txt";
        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonHelper.FromJson<Blueprint>(json);
            }
            else return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
