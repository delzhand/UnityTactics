using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetableActionState
{
    Inactive,
    WaitingForTileSelect,
    WaitingForTileConfirm,
}

public abstract class TargetableAction_MenuOption : MenuOption {
    public TargetableActionState State = TargetableActionState.Inactive;
    public CombatUnit Caster;

    private void Start()
    {
        Caster = Menu.Attach.GetComponent<CombatUnit>();
    }

    private void Update()
    {
        switch (State)
        {
            case TargetableActionState.WaitingForTileSelect:
                Engine.MapCursor.DelegatedInput();
                if (Engine.InputManager.Accept)
                {

                    Engine.InputManager.Accept = false;
                    Tile TargetTile = Engine.MapCursor.GetTile();
                    if (TargetTile.CurrentlySelectable)
                    {
                        TargetTile = Engine.MapCursor.GetTile();
                        HighlightAffectedTiles(TargetTile);
                        Caster.gameObject.GetComponent<UnitPanel>().ModeOverride = 1;
                        State = TargetableActionState.WaitingForTileConfirm;

                        if (TileOccupier.GetTileOccupant(TargetTile) != null)
                        {
                            CombatUnit target = TileOccupier.GetTileOccupant(TargetTile).GetComponent<CombatUnit>();
                            if (target)
                            {
                                PredictEffect(target);
                            }
                        }
                    }
                    else
                    {
                        Message m = new GameObject().AddComponent<Message>();
                        m.BoxMessageLabel = "Check";
                        m.BoxMessage = "Select within range.";
                        m.ReturnControlTo = this;
                        Engine.AssignControl(m);
                    }
                }
                else if (Engine.InputManager.Cancel)
                {
                    Engine.InputManager.Cancel = false;
                    State = TargetableActionState.Inactive;
                    Menu.ShowAllMenus();
                    Engine.AssignControl(Menu);
                    Engine.TileManager.ClearHighlights(); //Engine.PathManager.ClearAll(false);
                    Engine.CameraManager.SetTargetPosition(Caster.transform.position, .5f);
                    Engine.MapCursor.X = Caster.gameObject.GetComponent<TileOccupier>().X;
                    Engine.MapCursor.Y = Caster.gameObject.GetComponent<TileOccupier>().Y;
                    Engine.MapCursor.GoToTile();
                }
                break;
            case TargetableActionState.WaitingForTileConfirm:
                if (Engine.InputManager.Accept)
                {
                    Engine.InputManager.Accept = false;
                    Menu m = new GameObject().AddComponent<Menu>();
                    m.Attach = Caster.gameObject;
                    m.Top = 180;
                    m.Left = Screen.width - 360;
                    m.name = "Confirm Effect";
                    m.ReturnControlTo = this;
                    Execute_MenuOption e = m.gameObject.AddComponent<Execute_MenuOption>();
                    SetExecute(e);
                    m.AddMenuOption("Execute", e);
                    NoExecute_MenuOption ne = m.gameObject.AddComponent<NoExecute_MenuOption>();
                    m.AddMenuOption("Quit", ne);
                }
                if (Engine.InputManager.Cancel)
                {
                    Engine.InputManager.Cancel = false;
                    State = TargetableActionState.WaitingForTileSelect;

                    Engine.TileManager.ClearHighlights();
                    HighlightSelectableTiles();

                    //Prediction[] ps = ReturnControlTo.gameObject.GetComponents<Prediction>();
                    //foreach (Prediction p in ps)
                    //{
                    //    Destroy(p);
                    //}
                }
                break;
        }

    }

    public override void SelectOption()
    {
        base.SelectOption();
        StartTileSelection();
    }

    public void StartTileSelection()
    {
        Menu.HideAllMenus();
        Engine.TileManager.ClearHighlights();
        HighlightSelectableTiles();
        Engine.AssignControl(this);
        State = TargetableActionState.WaitingForTileSelect;
        Menu.Attach.GetComponent<ActiveTurn>().ReturnControlTo = Menu;
        Engine.MapCursor.PanelMode = 2;
    }

    public virtual void HighlightSelectableTiles() { }
    public virtual void HighlightAffectedTiles(Tile TargetTile) { }
    public virtual void PredictEffect(CombatUnit Target) { }
    public virtual void SetExecute(Execute_MenuOption e) { }
}
