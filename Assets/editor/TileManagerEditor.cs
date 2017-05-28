using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TileManager tileManager = (TileManager)target;

        if (GUILayout.Button("Update Tile Positions"))
        {
            foreach (Tile t in tileManager.GetComponentsInChildren<Tile>())
            {
                t.SetPosition();
            }

        }


        if (GUILayout.Button("Remove All Highlight Meshes"))
        {
            foreach(Tile t in tileManager.GetComponentsInChildren<Tile>())
            {
                t.HighlightGenerated = false;
                DestroyImmediate(t.GetComponent<MeshFilter>());
                DestroyImmediate(t.GetComponent<MeshRenderer>());
            }
        }
    }

}
