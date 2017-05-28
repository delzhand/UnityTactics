using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
[CanEditMultipleObjects]
public class TileEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Tile tile = (Tile)target;
        EditorGUILayout.LabelField("Path Parent: " + tile.BestParent.Key);

        if (GUILayout.Button("Raise"))
        {
            tile.Height++;
            tile.SetPosition();
        }
        if (GUILayout.Button("Lower"))
        {
            tile.Height--;
            tile.SetPosition();
        }
    }
}
