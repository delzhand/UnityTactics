using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Mover))]
public class MoverEditor : Editor {

    private int tileX;
    private int tileY;

    public override void OnInspectorGUI()
    {
        Mover mover = (Mover)target;
        mover.OriginPosition = EditorGUILayout.Vector3Field("Origin", mover.OriginPosition);
        if (GUILayout.Button("Copy position to origin"))
        {
            mover.OriginPosition = mover.transform.position;
        }
        tileX = EditorGUILayout.IntField("Origin Tile X", tileX);
        tileY = EditorGUILayout.IntField("Origin Tile X", tileY);
        if (GUILayout.Button("Get origin from tile"))
        {
            mover.OriginPosition = GameObject.Find("[" + tileX + "," + tileY + "]").transform.position;
        }

        EditorGUILayout.LabelField("Move Time: " + mover.MoveTime());
        EditorGUILayout.LabelField("Transition Move Time: " + (mover.Transition * mover.MoveTime()));
        EditorGUILayout.LabelField("Index, %: " + mover.GetCurrentWaypointIndex() + ", " + mover.GetCurrentWaypointTransition());
        //mover.Transition = EditorGUILayout.Slider("Transition", mover.Transition, 0, 1);
        //mover.UpdatePosition();

        if (GUILayout.Button("Play Forward"))
        {
            mover.PlayForward();
        }
        if (GUILayout.Button("Play Reverse"))
        {
            mover.PlayReverse();
        }
    }
}
