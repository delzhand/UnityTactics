using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class Weapon {
    public string Id;
    public string Type;
    public string AttackPattern = "Striking";
    public string PSXName;
    public string PSPName;
    public int Cost;
    public int WP;
    public int WEv;
    public int MinRange = 1;
    public int MaxRange = 1;

    public void HighlightSelectableTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        switch(AttackPattern)
        {
            case "Lunge":
                throw new NotImplementedException("Lunging weapon pattern not implemented.");
            case "LineOfSight":
                tiles = LineOfSightSelectable(origin);
                break;
            case "Parabola":
                foreach(Tile t in Engine.TileManager.AllTiles())
                {
                    int heightDiff = (int)(origin.GetEffectiveHeight() - t.GetEffectiveHeight())/2;
                    int range = heightDiff + 5;
                    if (Tile.GetDistance(t, origin) <= range)
                    {
                        tiles.Add(t);
                    }
                }
                foreach (Tile t in Engine.TileManager.FindTilesByRadius(origin, MinRange - 1, true))
                {
                    tiles.Remove(t);
                }

                break;
            default:
                foreach (Tile t in Engine.TileManager.FindTilesByRadius(origin, MaxRange, false))
                {
                    tiles.Add(t);
                }
                break;
        }
        foreach (Tile t in tiles)
        {
            Material m = Resources.Load("graphics/materials/utility/red_highlight", typeof(Material)) as Material;
            t.Highlight(m);
        }
    }

    private List<Tile> LineOfSightSelectable(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile[] maxRadius = Engine.TileManager.FindTilesByRadius(origin, MaxRange, false);
        Tile[] exclude = Engine.TileManager.FindTilesByRadius(origin, MinRange - 1, false);
        foreach (Tile t in maxRadius)
        {
            tiles.Add(t);
        }
        foreach (Tile t in exclude)
        {
            tiles.Remove(t);
        }
        return tiles;
    }
}
