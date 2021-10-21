using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVParser : MonoBehaviour
{
    private const string SPLIT_RE = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
    private const string LINE_SPLIT_RE = "[\n\r]";
    
    private static string ReadString(string path)
    {
        TextAsset csvTextAsset = Resources.Load(path) as TextAsset;
        return csvTextAsset.text;
    }
    private static string ReadString(UnityEngine.Object ob)
    {
        TextAsset csvTextAsset = ob as TextAsset;
        return csvTextAsset.text;
    }

    private static List<Dictionary<string, string>> ParseToDict(string str)
    {
        string[] dataLines = Regex.Split(str, LINE_SPLIT_RE);
        List<Dictionary<string, string>> Datas = new List<Dictionary<string, string>>();
        
        List<string> categories = new List<string>(Regex.Split(dataLines[0], SPLIT_RE));
        
        Dictionary<string, string> line;        
        int num = 1;
        string[] tokens;
        while (num < dataLines.Length)
        {
            tokens = Regex.Split(dataLines[num], SPLIT_RE);
            line = new Dictionary<string, string>();
            
            for (int i=0; i < categories.Count; i++)
            {
                line.Add(categories[i], tokens[i]);
            }
            Datas.Add(line);
            num++;
        }
        return Datas;
    }
    
    private static List<List<string>> ParseToList(string str)
    {
        string[] dataLines = Regex.Split(str, LINE_SPLIT_RE);
        List<List<string>> Datas = new List<List<String>>();
        
        List<string> categories = new List<string>(Regex.Split(dataLines[0], SPLIT_RE));
        
        List<string> line;        
        int num = 1;
        string[] tokens;
        while (num < dataLines.Length)
        {
            tokens = Regex.Split(dataLines[num], SPLIT_RE);
            line = new List<string>();
            
            for (int i=0; i < categories.Count; i++)
            {
                line.Add(tokens[i]);
            }
            Datas.Add(line);
            num++;
        }
        return Datas;
    }
    
    /// <summary>
    /// List<Dictionary<카테고리명,내용>>
    /// </summary>
    /// <returns></returns>
    public static List<Dictionary<string, string>> ReadAndParseToDict(string path)
    {
        return ParseToDict(ReadString(path));
    }
    /// <summary>
    /// List<List<내용>>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<List<string>> ReadAndParseToList(string path)
    {
        return ParseToList(ReadString(path));
    }
}
