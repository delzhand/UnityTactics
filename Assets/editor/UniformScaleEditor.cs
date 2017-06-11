using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UniformScale))]
[CanEditMultipleObjects]
public class UniformScaleEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UniformScale uniformScale = (UniformScale)target;
        uniformScale.transform.localScale = new Vector3(uniformScale.Scale, uniformScale.Scale, uniformScale.Scale);
    }

}
