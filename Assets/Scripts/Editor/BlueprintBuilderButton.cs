using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlueprintBuilder))]
public class BlueprintBuilderButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BlueprintBuilder builder = (BlueprintBuilder)target;
        if (GUILayout.Button("Build Data"))
        {
            builder.BuildData();
            AssetDatabase.Refresh();
        }
    }
}