using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoteHitTimings))]
public class NoteHitTimingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NoteHitTimings timings = (NoteHitTimings)target;

        if (GUILayout.Button("Parse Lists"))
        {
            timings.ParseAllLists();
            EditorUtility.SetDirty(timings);
        }
    }
}