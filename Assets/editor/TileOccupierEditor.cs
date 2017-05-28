using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileOccupier))]
public class TileOccupierEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset Position"))
        {
            TileOccupier tileOccupier = (TileOccupier)target;
            tileOccupier.transform.position = GameObject.Find("Tiles").GetComponent<TileManager>().FindTiles(tileOccupier.X, tileOccupier.Y)[tileOccupier.Index].transform.position;
        }
    }
}
