using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaypointType
{
    Walk,
    VerticalJumpUp,
    WalkOffDown,
    HorizontalJumpDown,
    HorizontalJump,
    Warp
}

public class Waypoint : MonoBehaviour {
    public int X;
    public int Y;
    public WaypointType Type;
    public float TimeMultiplier = 1;
    
    public void Initialize(int x, int y, WaypointType type, float timeMultiplier)
    {
        X = x;
        Y = y;
        Type = type;
        TimeMultiplier = timeMultiplier;
    }

    public override string ToString()
    {
        return "[" + X + "," + Y + "] " + Type.ToString() + " (" + TimeMultiplier + ")";
    }

    public Vector3 GetTargetPosition()
    {
        return GameObject.Find("[" + X + "," + Y + "]").GetComponent<Tile>().transform.position;
    }
}
