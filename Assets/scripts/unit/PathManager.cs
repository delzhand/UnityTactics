using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {

    public int MaxRange;
    public int MaxJump;
    public int OriginX;
    public int OriginY;
    public int TargetX;
    public int TargetY;

	void Start () {
        //MarkValidTiles();
	}

    private void OnDestroy()
    {
        ClearAll(true); 
    }

    public void ClearAll(bool fullClear = false)
    {
        TileManager tileManager = gameObject.GetComponent<TileManager>();
        foreach (Tile t in tileManager.GetComponentsInChildren<Tile>())
        {
            t.ClearHighlight();
            t.SearchNeighbors = 0;
            if (fullClear)
            {
                t.CurrentlySelectable = false;
                t.BestParent = new KeyValuePair<Tile, int>(null, 0);
            }
        }
    }

    public void HighlightPath()
    {
        List<Tile> path = GetPath();
        Material m = Resources.Load("graphics/materials/utility/path_highlight", typeof(Material)) as Material;
        foreach (Tile t in path)
        {
            t.Highlight(m);
        }
    }

    public List<Tile> GetPath()
    {
        List<Tile> path = new List<Tile>();
        TileManager tileManager = gameObject.GetComponent<TileManager>();
        ClearAll();
        Tile target = tileManager.FindTile(TargetX, TargetY);
        target.Highlight();
        path.Add(target);
        Tile origin = tileManager.FindTile(OriginX, OriginY);
        origin.Highlight();
        Tile parent = target.BestParent.Key;
        int maxLoop = 100;
        while (parent != null && parent != origin)
        {
            path.Add(parent);
            parent = parent.BestParent.Key;
            maxLoop--;
            if (maxLoop <= 0)
            {
                throw new Exception("Too many loops.");
            }
        }
        path.Add(origin);
        return path;
    }

    public void MarkValidTiles()
    {
        ClearAll(true);

        Tile origin = Engine.TileManager.FindTile(OriginX, OriginY);
        origin.SearchNeighbors = 1;

        for (int i = 0; i < MaxRange; i++)
        {
            SinglePass();
        }

        // Always remove the origin to avoid infinite parent chasing
        origin.BestParent = new KeyValuePair<Tile, int>(null, 0);
        origin.ClearHighlight();
        origin.CurrentlySelectable = false;


        // Now go through and remove all the tiles occupied by allied units (tiles occupied by enemy units shouldn't even be available)
        foreach (TileOccupier tO in GameObject.Find("Units").transform.GetComponentsInChildren<TileOccupier>())
        {
            Tile[] selectableTiles = Engine.TileManager.FindSelectableTiles();
            for (int i = 0; i < selectableTiles.Length; i++)
            {
                if (selectableTiles[i].X == tO.X && selectableTiles[i].Y == tO.Y && selectableTiles[i].ElevationIndex == tO.Index)
                {
                    selectableTiles[i].CurrentlySelectable = false;
                    selectableTiles[i].ClearHighlight();
                }
            }
        }

    }

    public void SinglePass()
    {
        List<Tile> tilesToCheck = new List<Tile>();
        foreach (Tile t in Engine.TileManager.GetComponentsInChildren<Tile>())
        {
            if (t.SearchNeighbors == 1)
            {
                tilesToCheck.Add(t);
            }
            if (t.SearchNeighbors > 0)
            {
                t.SearchNeighbors--;
            }
        }
        foreach (Tile t in tilesToCheck)
        {
            t.MarkTravelNeighbors(MaxRange, MaxJump);
            t.SearchNeighbors = 0;
        }

    }

}

