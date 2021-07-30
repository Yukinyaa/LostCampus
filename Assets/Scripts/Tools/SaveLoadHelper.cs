using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public static class SaveLoadHelper
{
    public static string SERVER_SAVE_PATH = Application.persistentDataPath + "/Server/";
    public static string Client_SAVE_PATH = Application.persistentDataPath + "/Client/";
    public static bool LocalSave(string str, string filename, string path)
    {
        try
        {
            using (FileStream file = new FileStream(path + filename, FileMode.Create))
            {
                using (BufferedStream buf = new BufferedStream(file))
                {
                    byte[] binary = Encoding.UTF8.GetBytes(str);
                    buf.Write(binary, 0, binary.Length);
                    return true;
                }
            }
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.LogWarning(e.Message);
            System.IO.Directory.CreateDirectory(path);
            return false;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }
    
    }

    public static bool LocalLoad(string filename, string path, out string data)
    {        
        try
        {
            using (FileStream file = new FileStream(path + filename , FileMode.OpenOrCreate))
            {
                using (BufferedStream buf = new BufferedStream(file))
                {
                    byte[] binary = new byte[buf.Length];
                    buf.Read(binary, 0, binary.Length);
                    if (binary.Length == 0)
                    {
                        Debug.LogWarning("no file data : " + path + filename);
                        data = null;
                        return false;
                    }
                    data = Encoding.UTF8.GetString(binary);
                    return true;
                }
            }
        }      
        catch (DirectoryNotFoundException e)
        {
            Debug.LogWarning(e.Message);
            System.IO.Directory.CreateDirectory(path);
            data = null;
            return false;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            data = null;
            return false;
        }
        
    }
}
