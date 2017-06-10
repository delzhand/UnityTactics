using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Appearance))]
public class AppearanceEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Appearance appearance = (Appearance)(target);

        if (GUILayout.Button("Configure"))
        {
            appearance.Configure();
        }

        if (GUILayout.Button("Cleanup"))
        {
            appearance.Cleanup();
        }
    }
}
