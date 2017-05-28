using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statics : MonoBehaviour {

    public static List<Tile> AffectedTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(origin);
        return tiles;
    }

    public static List<Tile> AffectedTiles(int radius, int vTolerance, Tile origin, bool includeOrigin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile[] range = Engine.TileManager.FindTilesByRadius(origin, radius, includeOrigin);
        foreach (Tile t in range)
        {
            if (Mathf.Abs(origin.Height - t.Height) <= vTolerance)
            {
                tiles.Add(t);
            }
        }
        return tiles;
    }

    public static void HighlightTiles(List<Tile> tiles)
    {
        Material m = Resources.Load("graphics/materials/utility/yellow_highlight", typeof(Material)) as Material;
        foreach (Tile t in tiles)
        {
            t.Highlight(m);
        }
    }

    public static Tile[] RadiusTiles(Tile tile, int radius, bool includeOrigin)
    {
        return Engine.TileManager.FindTilesByRadius(tile, radius, includeOrigin);
    }
}
