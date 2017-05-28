using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CameraManager cameraManager = (CameraManager)target;
        cameraManager.Transition = EditorGUILayout.Slider("Transition", cameraManager.Transition, 0, 1);
        cameraManager.UpdatePosition();

        if (GUILayout.Button("Play Forward"))
        {
            cameraManager.PlayForward();
        }
    }
}
