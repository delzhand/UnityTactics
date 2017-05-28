using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathfinderEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PathManager pathfinder = (PathManager)target;

        if (GUILayout.Button("Update"))
        {
            pathfinder.MarkValidTiles();
        }

        if (GUILayout.Button("Show Path"))
        {
            pathfinder.HighlightPath();
        }

        if (GUILayout.Button("Single Pass"))
        {
            pathfinder.SinglePass();
        }
    }
}
