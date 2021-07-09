using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MySetting
{
    [System.Serializable]
    public class MySettingData
    {
        public float bgm;
        public float sfx;
        public string username;

        public MySettingData()
        {
            bgm = 0.5f;
            sfx = 0.5f;
            username = "Player";
        }

        public override string ToString()
        {
            return $"BGM : {bgm}\nSFX : {sfx}\nUsername : {username}";
        }
    }
    private static MySettingData mySettingData;

    public static float BGM
    {
        get
        {
            if (mySettingData == null) Load();
            return mySettingData.bgm;
        }
        set
        {
            if (0 <= value && value <= 1f)
            {
                if (mySettingData == null) Load();
                mySettingData.bgm = value;
            }
        }
    }

    public static float SFX
    {
        get
        {
            if (mySettingData == null) Load();
            return mySettingData.sfx;
        }
        set
        {
            if (0 <= value && value <= 1f)
            {
                if (mySettingData == null) Load();
                mySettingData.sfx = value;
            }
        }
    }

    public static string UserName
    {
        get
        {
            if (mySettingData == null) Load();
            return mySettingData.username;
        }
        set
        {
            if (mySettingData == null) Load();
            mySettingData.username = value;
        }
    }

    public static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(mySettingData, true);
            string path = Application.persistentDataPath + "/" + "settings.dat";
            File.WriteAllText(path, json);
        }
        catch (IOException)
        {

        }
    }

    public static void Load()
    {
        try
        {
            string path = Application.persistentDataPath + "/" + "settings.dat";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                mySettingData = JsonUtility.FromJson<MySettingData>(json);

            }
            else
            {
                mySettingData = new MySettingData();
                string json = JsonUtility.ToJson(mySettingData, true);
                File.WriteAllText(path, json);
            }
        }
        catch (IOException)
        {

        }
    }


}
