using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(tmp))]
public class tmpEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        tmp tmp = (tmp)target;
    }

}
