using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaitState
{
    Resolving,
    None,
    WaitingForMovement,
    WaitingForMoveConfirm,
}

public class ActiveTurn : MonoBehaviour
{

    public bool Active = false;
    public int MovePoints;
    public int ActPoints;

    public Tile StartingTile;
    public Tile TargetTile;

    public WaitState WaitState;

    public MonoBehaviour ReturnControlTo;

    void Update()
    {
        if (Engine.InputManager.Attach == this)
        {
            if (GetComponent<CombatUnit>().Guest || GetComponent<CombatUnit>().Enemy)
            {
                Debug.Log("AI not implemented. Skipping turn.");
                endTurn();
            }

            switch (WaitState)
            {
                case WaitState.WaitingForMovement:
                    Engine.MapCursor.DelegatedInput();
                    if (Engine.InputManager.Accept)
                    {
                        Engine.InputManager.Accept = false;
                        TargetTile = Engine.MapCursor.GetTile();
                        if (TargetTile.CurrentlySelectable)
                        {
                            DoMove();
                        }
                        else
                        {
                            Message m = new GameObject().AddComponent<Message>();
                            m.BoxMessageLabel = "Check";
                            m.BoxMessage = "Select within movable range.";
                            m.ReturnControlTo = this;
                            Engine.AssignControl(m);
                        }
                    }
                    else if (Engine.InputManager.Cancel)
                    {
                        Engine.InputManager.Cancel = false;
                        Engine.AssignControl(ReturnControlTo);
                        Menu m = (Menu)ReturnControlTo;
                        m.Visible = true;
                        Engine.PathManager.ClearAll(false);
                        Engine.CameraManager.SetTargetPosition(transform.position, .5f);
                        Engine.MapCursor.X = GetComponent<TileOccupier>().X;
                        Engine.MapCursor.Y = GetComponent<TileOccupier>().Y;
                        Engine.MapCursor.GoToTile();
                    }
                    break;
                case WaitState.WaitingForMoveConfirm:
                    if (Engine.InputManager.Accept)
                    {
                        Engine.InputManager.Accept = false;
                        GetComponent<TileOccupier>().X = TargetTile.X;
                        GetComponent<TileOccupier>().Y = TargetTile.Y;
                        Engine.CameraManager.SetTargetPosition(transform.position, .5f);
                        Engine.PathManager.ClearAll(true);
                        MovePoints = 0;
                        Menu.DestroyAllMenus();
                        WaitState = WaitState.None;
                    }
                    else if (Engine.InputManager.Cancel)
                    {
                        Engine.InputManager.Cancel = false;
                        WaitState = WaitState.WaitingForMovement;
                        transform.position = GetComponent<TileOccupier>().GetOccupiedTile().transform.position;
                        GetComponent<Mover>().ShowMoveTiles();
                        foreach (Waypoint w in GetComponents<Waypoint>())
                        {
                            Destroy(w);
                        }
                    }
                    break;
                case WaitState.None:
                    if (MovePoints == 0 && ActPoints == 0)
                    {
                        GameObject facing_selector = Instantiate(Resources.Load("prefabs/units/Facing Selector")) as GameObject;
                        facing_selector.name = "Facing Selector";
                        facing_selector.transform.parent = transform;
                        facing_selector.transform.localPosition = Vector3.zero;
                        facing_selector.transform.localRotation = Quaternion.identity;
                        Engine.AssignControl(GetComponent<Facer>());
                    }
                    else
                    {
                        CombatUnit c = GetComponent<CombatUnit>();
                        if (!c.Guest && !c.Enemy)
                        {
                            c.ShowMenu();
                            Engine.MapCursor.Show();
                            Engine.MapCursor.PanelMode = 1;
                        }
                    }
                    break;
            }
        }
    }

    public void Activate()
    {
        Active = true;
        StartingTile = GetComponent<TileOccupier>().GetOccupiedTile();

        Engine.MapCursor.X = GetComponent<TileOccupier>().X;
        Engine.MapCursor.Y = GetComponent<TileOccupier>().Y;
        Engine.MapCursor.GoToTile();
        Engine.MapCursor.Show();

        MovePoints = 1;
        ActPoints = 1;

        KO k = GetComponent<KO>();
        if (k != null)
        {
            k.Counter--;
        }

        CombatUnit c = GetComponent<CombatUnit>();
        if (!c.Guest && !c.Enemy)
        {
            Engine.CameraManager.SetTargetPosition(transform.position, .5f);
        }

        WaitState = WaitState.None;
        Engine.AssignControl(this);
    }

    public void ShowPath(Tile target)
    {
        GetComponent<Mover>().PreparePath(target);
        WaitState = WaitState.WaitingForMoveConfirm;
    }

    public void DoMove()
    {
        GetComponent<Mover>().PreparePath(TargetTile);
        GetComponent<Mover>().PlayForward();
        WaitState = WaitState.Resolving;
        Engine.PathManager.ClearAll(false);
        Invoke("DoneMoving", GetComponent<Mover>().MoveTime());
    }

    public void DoneMoving()
    {
        WaitState = WaitState.WaitingForMoveConfirm;
    }

    public void DoneFacing()
    {
        Destroy(transform.Find("Facing Selector").gameObject);
        endTurn();
    }

    private void endTurn()
    {
        CombatUnit c = GetComponent<CombatUnit>();
        int ctExpended = 0;
        if (ActPoints == 0 && MovePoints == 0)
        {
            ctExpended = 100;
        }
        else if (ActPoints > 0)
        {
            ctExpended = 80;
        }
        else if (MovePoints > 0)
        {
            ctExpended = 80;
        }
        c.CT -= ctExpended;
        if (c.CT > 60)
        {
            c.CT = 60;
        }
        Destroy(this);
        Engine.AssignControl(null);
        Engine.CombatManager.Unpause();
        Menu.DestroyAllMenus();

    }
}
