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

    }
}

