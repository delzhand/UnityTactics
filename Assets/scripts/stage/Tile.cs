using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlopeType
{
    Flat_0,
    Incline_N,
    Incline_E,
    Incline_S,
    Incline_W,
    Convex_NE,
    Convex_SE,
    Convex_NW,
    Convex_SW,
    Concave_NE,
    Concave_SE,
    Concave_NW,
    Concave_SW
}

public enum Direction
{
    North,
    Northeast,
    East,
    Southeast,
    South,
    Southwest,
    West,
    Northwest
}

public enum Surface
{
    Book,
    Box,
    Brick,
    Bridge,
    Chimney,
    Coffin,
    Cross_Section,
    Darkness,
    Deck,
    Furniture,
    Grassland,
    Gravel,
    Ice,
    Iron_Plate,
    Ivy,
    Lake,
    Lava,
    Lava_Rocks,
    Machine,
    Marsh,
    Moss,
    Mud_Wall,
    Natural_Surface,
    Obstacle,
    Poisoned_Marsh,
    River,
    Road,
    Rocky_Cliff,
    Roof,
    Rug,
    Salt,
    Sand_Area,
    Sea,
    Sky,
    Snow,
    Stairs,
    Stalactite,
    Stone_Floor,
    Stone_Wall,
    Swamp,
    Thicket,
    Tombstone,
    Tree,
    Wasteland,
    Water_Plant,
    Waterfall,
    Waterway,
    Wooden_Floor
}

public class Tile : MonoBehaviour {

    public int X;
    public int Y;
    public int Height;
    public int Depth;
    public int Slope;
    public int ElevationIndex;
    public SlopeType SlopeType;
    public Surface Surface;
    public bool Impassable;
    public bool Unselectable;
    public bool HighlightGenerated = false;
    public int SearchNeighbors = 0;

    public KeyValuePair<Tile, int> BestParent;
    public bool CurrentlySelectable;

    void OnDrawGizmos()
    {
        drawGrid();    
    }

    void OnDrawGizmosSelected()
    {
        drawGrid(Color.green);
    }

    public string GetDisplayedHeight()
    {
        String s = (Height + Depth + Slope / 2f) + "h";
        if (Depth != 0)
        {
            s += "\ndepth " + Depth;
        }
        return s;
    }

    public void SetPosition()
    {
        Vector3 p = new Vector3(X, Height + Depth, Y);
        float yScale = transform.parent.GetComponent<TileManager>().TileHeightScale;
        transform.position = new Vector3(p.x, (p.y + Slope / 2f) * yScale, p.z);
    }

    public Vector3[] GetVertices()
    {
        Vector3 p = new Vector3(X, Height + Depth, Y);
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(p.x - .5f, p.y, p.z + .5f); //nw
        vertices[1] = new Vector3(p.x + .5f, p.y, p.z + .5f); //ne
        vertices[2] = new Vector3(p.x - .5f, p.y, p.z - .5f); //sw
        vertices[3] = new Vector3(p.x + .5f, p.y, p.z - .5f); //se

        switch (SlopeType)
        {
            case SlopeType.Incline_N:
                vertices[0].y += Slope;
                vertices[1].y += Slope;
                break;
            case SlopeType.Incline_S:
                vertices[2].y += Slope;
                vertices[3].y += Slope;
                break;
            case SlopeType.Incline_E:
                vertices[3].y += Slope;
                vertices[1].y += Slope;
                break;
            case SlopeType.Incline_W:
                vertices[2].y += Slope;
                vertices[0].y += Slope;
                break;
            case SlopeType.Concave_NE:
                vertices[0].y += Slope;
                vertices[1].y += Slope;
                vertices[3].y += Slope;
                break;
            case SlopeType.Concave_NW:
                vertices[0].y += Slope;
                vertices[1].y += Slope;
                vertices[2].y += Slope;
                break;
            case SlopeType.Concave_SE:
                vertices[2].y += Slope;
                vertices[3].y += Slope;
                vertices[1].y += Slope;
                break;
            case SlopeType.Concave_SW:
                vertices[2].y += Slope;
                vertices[3].y += Slope;
                vertices[0].y += Slope;
                break;
            case SlopeType.Convex_NE:
                vertices[1].y += Slope;
                break;
            case SlopeType.Convex_NW:
                vertices[0].y += Slope;
                break;
            case SlopeType.Convex_SE:
                vertices[3].y += Slope;
                break;
            case SlopeType.Convex_SW:
                vertices[2].y += Slope;
                break;
        }

        float yScale = transform.parent.GetComponent<TileManager>().TileHeightScale;
        vertices[0].y *= yScale;
        vertices[1].y *= yScale;
        vertices[2].y *= yScale;
        vertices[3].y *= yScale;

        return vertices;
    }

    private void drawGrid(Color? color = null)
    {
        if (color == null)
        {
            color = Impassable ? Color.red : new Color(.8f, .8f, 1f);
        }

        color = SearchNeighbors > 1 ? Color.cyan : color;
        color = SearchNeighbors == 1 ? Color.yellow : color;
        Gizmos.color = (Color)color;

        Vector3[] vertices = GetVertices();

        // Draw diagonals
        switch (SlopeType)
        {
            case SlopeType.Concave_NE:
            case SlopeType.Convex_NE:
            case SlopeType.Concave_SW:
            case SlopeType.Convex_SW:
                drawTileGizmo(vertices[1], vertices[2]);
                break;
            case SlopeType.Concave_NW:
            case SlopeType.Concave_SE:
            case SlopeType.Convex_NW:
            case SlopeType.Convex_SE:
                drawTileGizmo(vertices[3], vertices[0]);
                break;
        }

        // Draw edges   
        drawTileGizmo(vertices[0], vertices[1]);
        drawTileGizmo(vertices[2], vertices[3]);
        drawTileGizmo(vertices[0], vertices[2]);
        drawTileGizmo(vertices[1], vertices[3]);
    }

    private void drawTileGizmo(Vector3 a, Vector3 b)
    {
        Gizmos.DrawLine(a, b);
    }

    //public void ToggleHighlight()
    //{
    //    if (!HighlightGenerated)
    //    {
    //        Highlight();
    //    }
    //    else
    //    {
    //        gameObject.GetComponent<MeshRenderer>().enabled = !gameObject.GetComponent<MeshRenderer>().enabled;
    //    }
    //}

    public void Highlight()
    {
        Material m = Resources.Load("graphics/materials/utility/movable_highlight", typeof(Material)) as Material;
        Highlight(m);
    }

    public void Highlight(String s)
    {
        Material m = Resources.Load("graphics/materials/utility/" + s, typeof(Material)) as Material;
        Highlight(m);
    }

    public void Highlight(Material material)
    {
        // The first time we highlight a tile, generate it's mesh. After that we can toggle on/off the meshrenderer
        if (!HighlightGenerated)
        {
            Vector3[] vertices = GetVertices();
            // convert to local space and add slight offset for visual clarity
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= transform.position;
                vertices[i].y += .001f;
            }

            Vector2[] uvs = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
            int[] triangles_cw = new int[] { 0, 1, 2, 1, 3, 2 };
            int[] triangles_ccw = new int[] { 0, 3, 2, 0, 1, 3 };

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            mesh.vertices = vertices;
            mesh.uv = uvs;
            switch (SlopeType)
            {
                case SlopeType.Convex_SW:
                case SlopeType.Convex_NE:
                case SlopeType.Concave_NE:
                case SlopeType.Concave_SW:
                    mesh.triangles = triangles_cw;
                    break;
                default:
                    mesh.triangles = triangles_ccw;
                    break;
            }

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            HighlightGenerated = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
    }

    public void ClearHighlight()
    {
        if (HighlightGenerated)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public float HeightAtEdge(Direction d)
    {
        if (Slope == 0)
        {
            return Height;
        }

        if (
            ((SlopeType == SlopeType.Concave_NE || SlopeType == SlopeType.Convex_NE) && (d == Direction.North || d == Direction.East)) ||
            ((SlopeType == SlopeType.Concave_SE || SlopeType == SlopeType.Convex_SE) && (d == Direction.South || d == Direction.East)) ||
            ((SlopeType == SlopeType.Concave_NW || SlopeType == SlopeType.Convex_NW) && (d == Direction.North || d == Direction.West)) ||
            ((SlopeType == SlopeType.Concave_SW || SlopeType == SlopeType.Convex_SW) && (d == Direction.South || d == Direction.West)) ||
            (SlopeType == SlopeType.Incline_N && d == Direction.North) ||
            (SlopeType == SlopeType.Incline_S && d == Direction.South) ||
            (SlopeType == SlopeType.Incline_E && d == Direction.East) ||
            (SlopeType == SlopeType.Incline_W && d == Direction.West)
        )
        {
            return Height + Slope;
        }
        else
        {
            return Height;
        }
    }

    public override string ToString()
    {
        return "[" + X + "," + Y + "]";
    }

    //public void MarkNeighbors(int MaxRange, int MaxJump)
    //{
    //    List<KeyValuePair<Tile, int>> neighbors = GetNeighbors(MaxRange, MaxJump);
    //    foreach(KeyValuePair<Tile, int> kvp in neighbors) 
    //    {
    //        Tile t = (Tile)kvp.Key;
    //        t.Highlight();
    //        t.SearchNeighbors = true;
    //        if (BestParent.Key == null || kvp.Value < BestParent.Value)
    //        {
    //            t.BestParent = kvp;
    //        }
    //    }
    //}

    public void MarkTravelNeighbors(int MaxRange, int MaxJump)
    {
        Direction[] checkDirections = new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West };

        for (int i = 0; i < checkDirections.Length; i++)
        {
            Direction d = checkDirections[i]; // direction to check
            Direction di = checkDirections[(i + 2) % 4]; // direction inverse for edge

            int counter = 1;
            
            // Tiles in straight line
            while (counter <= MaxRange)
            {
                Tile[] tiles = transform.parent.GetComponent<TileManager>().FindTilesInDirection(this, d, counter);

                // Multiple tiles at same height
                foreach (Tile t in tiles)
                {
                    if (t == null)
                    {
                        // If there's no tile or the tile is unselectable, pass over it
                        continue;
                    }

                    // Gather some values
                    int occupied = TileOccupier.IsTileOccupied(t);
                    float difference = Height - t.HeightAtEdge(di);
                    int cost = (int)Math.Abs(difference) + counter;
                    float walkThreshold = 2f;

                    // Determine some conditions
                    bool enemyOccupied = (occupied == 1);
                    bool canWalk = Math.Abs(difference) <= walkThreshold;
                    bool canJumpV = Math.Abs(difference) <= MaxJump;
                    bool canJumpH = ((counter - 1) <= MaxJump / 2);
                    bool terrainAllowed = true;

                    // Status
                    bool stop = false;
                    bool mark = false;

                    //// Check adjacent tiles on the next pass
                    //if (counter == 1 && !t.Impassable && !enemyOccupied)
                    //{
                    //    t.SearchNeighbors = true;
                    //}

                    // If the unit can't pass to this tile, stop and don't mark
                    if (t.Impassable)
                    {
                        stop = true;
                    }

                    // If the unit can walk to a tile, stop and mark
                    else if (counter == 1 && canWalk && terrainAllowed && !enemyOccupied)
                    {
                        stop = true;
                        mark = true;
                    }

                    // If the unit can jump up to the tile, stop and mark
                    else if (!enemyOccupied && counter == 1 && difference < 0 && canJumpV)
                    {
                        stop = true;
                        mark = true;
                    }

                    // If the unit can jump horizontally, stop and mark
                    else if (!enemyOccupied && canJumpH && difference == 0)
                    {
                        stop = true;
                        mark = true;
                    }

                    // If we can't jump horizontally this far, stop and don't mark
                    else if (counter > MaxJump / 2)
                    {
                        stop = true;
                    }

                    // If we can't travel this far, stop and don't mark
                    else if (counter > MaxRange)
                    {
                        stop = true;
                    }

                    // If the tile is impassable, stop and don't mark
                    else if (t.Impassable)
                    {
                        stop = true;
                    }

                    // If occupied by an enemy, stop and don't mark
                    else if (enemyOccupied)
                    {
                        stop = true;
                    }

                    // If we're obstructed by a barrier we can't jump up to, stop and don't mark
                    else if (!canJumpV && difference < 0)
                    {
                        stop = true;
                    }

                    // If the unit can jump down, continue and mark
                    else if (canJumpV && canJumpH && difference > 0)
                    {
                        mark = true;
                    }

                    // Do the actual work of marking/stopping
                    if (mark)
                    {
                        MarkTile(t, cost);
                        t.SearchNeighbors = counter;
                    }
                    if (stop)
                    {
                        counter += 1000; // arbitrarily high number
                    }
                }

                counter++;
            }
        }
    }

    public void MarkTile(Tile t, int cost)
    {
        t.CurrentlySelectable = true;
        t.Highlight();
        if (t.BestParent.Key == null)
        {
            t.BestParent = new KeyValuePair<Tile, int>(this, cost);
        }
    }
}
