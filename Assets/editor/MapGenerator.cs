using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerator : EditorWindow
{
    private int height;
    private int width;

    [MenuItem("Window/MapGenerator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapGenerator));
    }

    void OnGUI()
    {
        height = EditorGUILayout.IntSlider("Height", height, 8, 24);
        width = EditorGUILayout.IntSlider("Width", width, 8, 24);


        if (GUILayout.Button("Build " + width + "x" + height + " Map"))
        {
            GameObject parent = new GameObject("Tiles");
            parent.AddComponent<PathManager>();
            TileManager tileManager = parent.AddComponent<TileManager>();
            tileManager.StageLength = height;
            tileManager.StageWidth = width;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    GameObject tileObject = new GameObject();
                    Tile t = tileObject.AddComponent<Tile>();
                    t.X = i;
                    t.Y = j;
                    tileObject.name = "[" + i + "," + j + "]";
                    tileObject.transform.parent = parent.transform;
                }
            }
        }

        if (GUILayout.Button("Initialize Collision"))
        {
            GameObject parent = GameObject.Find("Tiles");
            foreach(Transform t in parent.transform)
            {
                BoxCollider b = t.GetComponent<BoxCollider>();
                if (b == null)
                {
                    b = t.gameObject.AddComponent<BoxCollider>();
                }

                Tile tile = t.GetComponent<Tile>();

                float height = (tile.Height + tile.Depth + (tile.Slope/2f)) * parent.GetComponent<TileManager>().TileHeightScale;

                b.size = new Vector3(1, height, 1);
                b.center = new Vector3(0, -height / 2f, 0);
            }
        }

    }
}

