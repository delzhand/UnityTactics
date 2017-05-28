using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapCursor))]
public class MapCursorEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapCursor mapCursor = (MapCursor)target;
        if (GUILayout.Button("Show"))
        {
            mapCursor.Show();
        }
        if (GUILayout.Button("Hide"))
        {
            mapCursor.Hide();
        }
    }
}
