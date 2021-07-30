using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyItemDataBuilder))]
public class MyItemDataBuilderButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MyItemDataBuilder builder = (MyItemDataBuilder)target;
        if (GUILayout.Button("Build Data"))
        {
            builder.BuildData();
            AssetDatabase.Refresh();
        }
    }
}