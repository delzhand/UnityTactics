using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    public float TileHeightScale = 1;
    public int StageWidth = 8;
    public int StageLength = 8;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(.8f, .8f, 1f);
        Gizmos.DrawLine(new Vector3(-.5f, 0, -.5f), new Vector3(StageWidth - .5f, 0, -.5f));
        Gizmos.DrawLine(new Vector3(-.5f, 0, StageLength - .5f), new Vector3(StageWidth - .5f, 0, StageLength - .5f));
        Gizmos.DrawLine(new Vector3(-.5f, 0, -.5f), new Vector3(-.5f, 0, StageLength - .5f));
        Gizmos.DrawLine(new Vector3(StageWidth - .5f, 0, -.5f), new Vector3(StageWidth - .5f, 0, StageLength - .5f));
    }

    public List<Tile> AllTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach(Transform child in transform)
        {
            tiles.Add(child.GetComponent<Tile>());
        }
        return tiles;
    }

    public Tile FindTile(int x, int y, int elevationIndex = 0)
    {
        Tile[] tiles = FindTiles(x, y);
        return tiles[elevationIndex];
    }

    public Tile[] FindTiles(int x, int y)
    {
        Tile[] tiles = new Tile[2];
        foreach(Transform child in transform)
        {
            if (child.name == "[" + x + "," + y + "]")
            {
                tiles[child.GetComponent<Tile>().ElevationIndex] = child.GetComponent<Tile>();
            }
        }

        return tiles;
    }

    public Tile[] FindTilesByRadius(Tile origin, int radius, bool includeOrigin)
    {
        List<Tile> tiles = new List<Tile>();
        foreach(Transform child in transform)
        {
            int distance = Math.Abs(child.GetComponent<Tile>().X - origin.X) + Math.Abs(child.GetComponent<Tile>().Y - origin.Y);
            if (distance == 0 && includeOrigin)
            {
                tiles.Add(child.GetComponent<Tile>());
            }
            else if (distance <= radius && distance > 0)
            {
                tiles.Add(child.GetComponent<Tile>());
            }
        }
        return tiles.ToArray();
    }

    public Tile[] FindSelectableTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Tile>().BestParent.Key != null)
            {
                tiles.Add(child.GetComponent<Tile>());
            }
        }
        return tiles.ToArray();
        //Tile[] tiles = new Tile[2];
        //foreach (Transform child in transform)
        //{
        //    if (child.GetComponent<Tile>().BestParent.Key != null)
        //    {
        //        tiles[child.GetComponent<Tile>().ElevationIndex] = child.GetComponent<Tile>();
        //    }
        //}
        //
        //return tiles;
    }


    public Tile[] FindTilesInDirection(Tile origin, Direction d, int distance)
    {
        int x = origin.X;
        int y = origin.Y;
        switch(d)
        {
            case Direction.North:
                y += distance;
                break;
            case Direction.South:
                y -= distance;
                break;
            case Direction.East:
                x += distance;
                break;
            case Direction.West:
                x -= distance;
                break;
        }

        return FindTiles(x, y);
    }

    public void ClearHighlights()
    { 
        foreach(Tile t in GetComponentsInChildren<Tile>())
        {
            t.ClearHighlight();
        }
    }
}
