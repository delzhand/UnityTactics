using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOccupier : MonoBehaviour
{
    public int X;
    public int Y;
    public int Index;
    public bool Allied = true;

    public Tile GetOccupiedTile()
    {
        return Engine.TileManager.FindTile(X, Y, Index);
    }

    public static int IsTileOccupied(Tile t)
    {
        foreach (TileOccupier tO in GameObject.Find("Units").transform.GetComponentsInChildren<TileOccupier>())
        {
            if (tO.X == t.X && tO.Y == t.Y && tO.Index == t.ElevationIndex)
            {
                return tO.Allied ? -1 : 1;
            }
        }
        return 0;
    }

    public static TileOccupier GetTileOccupant(Tile t)
    {
        foreach (TileOccupier tO in GameObject.Find("Units").transform.GetComponentsInChildren<TileOccupier>())
        {
            if (tO.X == t.X && tO.Y == t.Y && tO.Index == t.ElevationIndex)
            {
                return tO;
            }
        }
        return null;
    }
}
