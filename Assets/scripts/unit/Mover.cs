using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public float Transition = 0;
    public Vector3 OriginPosition;
    private float playSpeed = 0;

    public float Timer = 0;

    public static float BaseMoveTime = .25f;

    void Start()
    {
        playSpeed = 0;
    }

    void Update()
    {
        if (playSpeed == 0)
        {
            return;
        }
        float moveTime = MoveTime();
        Timer += (playSpeed * Time.deltaTime);
        if (Timer > moveTime || Timer < 0)
        {
            playSpeed = 0;
        }
        Transition = Mathf.Clamp(Timer / moveTime, 0, 1);
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (Waypoints().Length == 0)
        {
            return;
        }

        int index = GetCurrentWaypointIndex();
        float waypointTransition = GetCurrentWaypointTransition();
        Vector3 waypointOriginPosition = (index > 0) ? Waypoints()[index - 1].GetTargetPosition() : OriginPosition;
        transform.position = getPosition(waypointOriginPosition, Waypoints()[index].GetTargetPosition(), waypointTransition, Waypoints()[index].Type);

        if (gameObject.GetComponent<Facer>())
        {
            gameObject.GetComponent<Facer>().UpdateFacing();
        }

        foreach (TileOccupier tO in GameObject.Find("Units").transform.GetComponentsInChildren<TileOccupier>())
        {
            if (tO.gameObject != gameObject && Vector3.Distance(tO.transform.position, transform.position) < .2f && tO.GetComponent<Shimmy>() == null)
            {
                Shimmy s = tO.gameObject.AddComponent<Shimmy>();
                if (transform.eulerAngles.y == 270)
                {
                    s.direction = 0;
                }
                else if (transform.eulerAngles.y == 180)
                {
                    s.direction = 1;
                }
                else if (transform.eulerAngles.y == 90)
                {
                    s.direction = 1;
                }
                else if (transform.eulerAngles.y == 0)
                {
                    s.direction = 3;
                }
            }
        }

    }

    public void PlayForward()
    {
        Timer = 0;
        playSpeed = 1f;
    }

    public void PlayReverse()
    {
        Timer = MoveTime();
        playSpeed = -1f;
    }

    public Waypoint[] Waypoints()
    {
        return gameObject.GetComponents<Waypoint>();
    }

    public float MoveTime()
    {
        float totalMoveTime = 0;
        Waypoint[] waypoints = Waypoints();
        foreach(Waypoint w in waypoints)
        {
            totalMoveTime += (BaseMoveTime * w.TimeMultiplier);
        }
        return totalMoveTime;
    }

    public int GetCurrentWaypointIndex()
    {
        float segmentStart = 0;
        for (int i = 0; i < Waypoints().Length; i++)
        {
            segmentStart += BaseMoveTime * Waypoints()[i].TimeMultiplier;
            if (Transition * MoveTime() <= segmentStart)
            {
                return i;
            }
        }
        return 0;
    }

    public float GetCurrentWaypointTransition()
    {
        float start = 0;
        float end = 0;
        for (int i = 0; i < Waypoints().Length; i++)
        {
            start = end;
            end = end + BaseMoveTime * Waypoints()[i].TimeMultiplier;
            if (Transition * MoveTime() <= end)
            {
                float elapsedInSegment = Transition * MoveTime() - start;
                float segmentLength = end - start;
                if (segmentLength == 0)
                {
                    return 0;
                }
                return elapsedInSegment/segmentLength;
            }
        }
        return 0;
    }

    public void PreparePath(Tile target)
    {
        Engine.PathManager.TargetX = target.X;
        Engine.PathManager.TargetY = target.Y;
        List<Tile> path = Engine.PathManager.GetPath();
        path.Reverse();
        Tile[] pathArray = path.ToArray();

        for(int i = 0; i < pathArray.Length; i++)
        {
            Tile t = pathArray[i];
            Waypoint w = gameObject.AddComponent<Waypoint>();
            w.X = t.X;
            w.Y = t.Y;
            if (i == 0)
            {
                w.TimeMultiplier = 0;
            }
            else
            {
                Tile pt = pathArray[i - 1]; // previous tile
                float heightDiff = pt.Height - t.Height;
                float horizontalDiff = Math.Abs(pt.X - t.X) + Math.Abs(pt.Y - t.Y);
                if (heightDiff < -2 )
                {
                    w.Type = WaypointType.VerticalJumpUp;
                }
                else if (heightDiff > 0)
                {
                    w.Type = WaypointType.WalkOffDown;
                }
                else if (heightDiff == 0 && horizontalDiff > 1)
                {
                    w.Type = WaypointType.HorizontalJump;
                    w.TimeMultiplier = 1;
                }
                
            }
        }
        OriginPosition = transform.position;
    }

    private Vector3 getPosition(Vector3 origin, Vector3 target, float progress, WaypointType type)
    {
        Vector3 position = Vector3.Lerp(origin, target, progress);

        float arcHeight = 0f;
        switch(type)
        {
            case WaypointType.HorizontalJump:
            case WaypointType.HorizontalJumpDown:
                arcHeight = .5f;
                position.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;
                break;
            case WaypointType.WalkOffDown:
                if (progress <= .5f)
                {
                    position.y = origin.y;
                }
                else
                {
                    position.y = Mathf.Lerp(origin.y, target.y, (progress - .5f)*2);
                }
                break;
            case WaypointType.VerticalJumpUp:
                float yDiff = target.y - origin.y;
                arcHeight = yDiff/2 + .5f;
                float subProgress = Mathf.Clamp(progress * 1.5f, 0, 1);
                position.y = Mathf.Lerp(origin.y, target.y, subProgress);
                position.y += Mathf.Sin(subProgress * Mathf.PI) * arcHeight;
                break;
        }

        return position;
    }

    public void ShowMoveTiles()
    {
        PathManager p = Engine.PathManager;
        p.MaxRange = GetComponent<CombatUnit>().Move;
        p.MaxJump = GetComponent<CombatUnit>().Jump;
        p.OriginX = GetComponent<TileOccupier>().X;
        p.OriginY = GetComponent<TileOccupier>().Y;
        p.MarkValidTiles();
    }

    public void ClearWaypoints()
    {
        Waypoint[] wps = GetComponents<Waypoint>();
        for(int i = 0; i<wps.Length; i++)
        {
            Destroy(wps[i]);
        }
    }

}
