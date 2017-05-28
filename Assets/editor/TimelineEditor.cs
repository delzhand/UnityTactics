using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Timeline))]
public class TimelineEditor : Editor {
    public override void OnInspectorGUI()
    {
        Timeline timeline = (Timeline)target;

        if (GUILayout.Button("Play"))
        {
            timeline.PlayFromStart();
        }

        if (GUILayout.Button("Destroy"))
        {
            Destroy(timeline.gameObject);
        }
    }
}
