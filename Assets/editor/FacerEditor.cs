using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Facer))]
public class FacerEditor : Editor {

    public override void OnInspectorGUI()
    {
        Facer facer = (Facer)target;
        foreach(Direction d in Enum.GetValues(typeof(Direction))){
            if (GUILayout.Button("Face " + d.ToString()))
            {
                facer.FaceDirection(d);
            }
        }

    }

}
