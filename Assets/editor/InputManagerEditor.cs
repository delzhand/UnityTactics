using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InputManager inputManager = (InputManager)target;

        EditorGUILayout.LabelField("Control: " + (inputManager.Attach != null ? inputManager.Attach.ToString() : "---"));

        //EditorGUILayout.LabelField("Horizontal: " + Input.GetAxis("Horizontal"));
        //EditorGUILayout.LabelField("Vertical: " + Input.GetAxis("Vertical"));
        //EditorGUILayout.LabelField("Tilt: " + Input.GetAxis("Tilt"));
        //EditorGUILayout.LabelField("Accept: " + Input.GetAxis("Accept"));
        //EditorGUILayout.LabelField("Cancel: " + Input.GetAxis("Cancel"));

    }

}
