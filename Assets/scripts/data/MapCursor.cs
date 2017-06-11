using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCursor : MonoBehaviour {
    public int X;
    public int Y;
    public int Index;

    private float timer = 0;
    public bool ShowHeight = false;

    public Direction UpDirection;

    public int PanelMode = 0;

    void OnGUI()
    {
        if (ShowHeight)
        {
            GUI.Label(new Rect(Screen.width - 60, 10, 50, 30), GetTile().GetDisplayedHeight());
        }
    }

    // Use this for initialization
    void Start () {
        Hide();
        GoToTile();
        //MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = Resources.Load("graphics/materials/utility/cursor_highlight", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update () {
        animate();

        if (Engine.InputManager.Attach == this)
        {
            DelegatedInput();

            if (Engine.InputManager.Accept)
            {
                Engine.InputManager.Accept = false;
                TileOccupier tO = TileOccupier.GetTileOccupant(GetTile());
                if (tO)
                {
                    tO.GetComponent<CombatUnit>().ShowMenu();
                }
            }
        }

    }

    public void DelegatedInput()
    {
        int originalX = X;
        int originalY = Y;

        Direction? d = Engine.InputManager.DirectionFromInput();
        if (d != null)
        {
            GoDirection((Direction)d);
        }
        else
        {
            return;
        }

        try
        {
            GoToTile();
            Engine.SoundManager.PlaySound("cursor_change");
        }
        catch (Exception)
        {
            // Don't move out of bounds
            X = originalX;
            Y = originalY;
        }
    }

    private void GoDirection(Direction d)
    {
        switch (d)
        {
            case Direction.North:
                Y++;
                break;
            case Direction.South:
                Y--;
                break;
            case Direction.East:
                X++;
                break;
            case Direction.West:
                X--;
                break;
            default:
                Debug.Log("Can't move in " + d + " direction.");
                break;
        }
    }

    private void animate()
    {
        timer += Time.deltaTime * 10;
        int seconds = Mathf.FloorToInt(timer) % 4;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            GetComponent<MeshFilter>().mesh.uv = new Vector2[] { new Vector2(.25f * (seconds - 1), 0), new Vector2(.25f * (seconds - 1), 1), new Vector2(.25f * seconds, 0), new Vector2(.25f * seconds, 1) };
        }
    }

    public Tile GetTile()
    {
        return Engine.TileManager.FindTiles(X, Y)[Index];
    }

    public void GoToTile()
    {
        transform.position = GetTile().transform.position;
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        bool newMesh = false;
        MeshFilter mf = GetComponent<MeshFilter>();
        if (!mf)
        {
            newMesh = true;
            mf = gameObject.AddComponent<MeshFilter>();
        }

        Tile t = Engine.TileManager.FindTiles(X, Y)[Index];

        Vector3[] vertices = t.GetVertices();
        // convert to local space and add slight offset for visual clarity
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= transform.position;
            vertices[i].y += .002f;
        }

        int[] triangles_cw = new int[] { 0, 1, 2, 1, 3, 2 };
        int[] triangles_ccw = new int[] { 0, 3, 2, 0, 1, 3 };

        Mesh mesh = mf.mesh;
        mesh.vertices = vertices;
        if (newMesh)
        {
            Vector2[] uvs = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(.25f, 0), new Vector2(.25f, 1) };
            mesh.uv = uvs;
        }
        switch (t.SlopeType)
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

    }

    public void Show()
    {
        ShowHeight = true;
        GetComponent<MeshRenderer>().enabled = true;
        transform.Find("UnitCursor").gameObject.SetActive(true);
    }

    public void Hide()
    {
        ShowHeight = false;
        GetComponent<MeshRenderer>().enabled = false;
        transform.Find("UnitCursor").gameObject.SetActive(false);
    }

}
