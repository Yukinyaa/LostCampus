#define TEST_DROP
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct DropTable
{
    public string name;
    public Dictionary<string, float> dropDict;

    public DropTable(string _name, Dictionary<string, float> _dropDict)
    {
        name = _name;
        dropDict = _dropDict;
    }
}

public static class DropTableDataBase
{
    public const string DROPTABLE_PATH = "DropTable.csv";

    private static List<DropTable> dropTables;
    public static List<DropTable> DropTables 
    {
        get
        {
            if (dropTables == null)
            {
                dropTables = LoadItemInfoFromDB(DROPTABLE_PATH);
            }

            return dropTables;
        }
    }

    public static DropTable FindDropTable(string id)
    {
        return dropTables.Find(x => x.name.Equals(id));
    }
   
    public static void AddDropTable(DropTable dropTable)
    {
        DropTables.Add(dropTable);
    }
   
    //TODO: 기획 정해지고 csv 파싱하기
    public static List<DropTable> LoadItemInfoFromDB(string path)
    {
#if TEST_DROP
        List<DropTable> tables = new List<DropTable>();
        DropTable table = new DropTable("ant", new Dictionary<string, float>() {{"1", 1.0f}});
        tables.Add(table);
        return tables;
#else
        List<Dictionary<String, String>> parseResult = CSVParser.ReadAndParseToDict(path);
        List<DropTable> tables = new List<DropTable>();
        
        foreach (var dict in parseResult)
        {
            string[] items = dict["item"].Split('/');
            string[] probs = dict["probability"].Split('/');
            Dictionary<string, float> dropPair = new Dictionary<string, float>();
            
            for (int i = 0; i < items.Length; i++)
            {
                dropPair.Add(items[i], float.Parse(probs[i]));
            }

            tables.Add(new DropTable(dict["name"], dropPair));
        }
        return tables;
#endif
    }
}
