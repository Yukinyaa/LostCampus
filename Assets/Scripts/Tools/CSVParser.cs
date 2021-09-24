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

    public static List<Dictionary<String, String>> Parse(string str)
    {
        string[] dataLines = Regex.Split(str, LINE_SPLIT_RE);
        List<Dictionary<String, String>> Datas = new List<Dictionary<String, String>>(); //첫째줄은 항목명으로 사용하니 0은 빈 리스트, 1부터 시작함
        
        List<String> categories = new List<string>();
        Dictionary<String, String> line;        
        int num = 1;
        string[] tokens;
        
        Datas.Add(new Dictionary<String, String>());
        while (num < dataLines.Length)
        {
            tokens = Regex.Split(dataLines[num], SPLIT_RE);
            line = new Dictionary<String, String>();
            
            for (int i=0; i < categories.Count; i++)
            {
                line.Add(categories[i], tokens[i]);
            }
            Datas.Add(line);
            num++;
        }
        return Datas;
    }
    
    /// <summary>
    /// List<Dictionary<항목명,내용>>
    /// </summary>
    /// <returns></returns>
    public static List<Dictionary<String, String>> ReadAndParse(string path)
    {
        return Parse(ReadString(path));
    }
}
