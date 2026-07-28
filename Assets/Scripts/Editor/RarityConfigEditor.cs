using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RarityConfig))]
public class RarityConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RarityConfig config = (RarityConfig)target;
        if (GUILayout.Button("Auto-Generate Peak/Spread"))
        {
            config.AutoGenerateWeights();
            EditorUtility.SetDirty(config); // marks the asset as changed so Unity saves it
        }
    }
}