using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public enum ActionOptionState
{
    Inactive,
    WaitingForTileSelect,
    WaitingForTileConfirm,
}

public class Action_MenuOption : MenuOption
{

    public string Action;
    public ActionOptionState State = ActionOptionState.Inactive;

    private void Start()
    {
        Action a = Engine.CombatManager.ActionTable[Action];
        CombatUnit caster = Menu.Attach.GetComponent<CombatUnit>();
        valid = a.CanBeCast(caster);
    }

    private void Update()
    {
        Action a = Engine.CombatManager.ActionTable[Action];
        CombatUnit caster = Menu.Attach.GetComponent<CombatUnit>();

        if (Engine.InputManager.Attach != this)
        {
            return;
        }

        switch (State)
        {
            case ActionOptionState.WaitingForTileSelect:
                Engine.MapCursor.DelegatedInput();
                if (Engine.InputManager.Accept)
                {

                    Engine.InputManager.Accept = false;
                    Tile TargetTile = Engine.MapCursor.GetTile();
                    if (TargetTile.CurrentlySelectable)
                    {
                        Engine.InputManager.Accept = false;

                        TargetTile = Engine.MapCursor.GetTile();
                        a.HighlightAffectedTiles(TargetTile, caster);
                        caster.gameObject.GetComponent<UnitPanel>().ModeOverride = 1;
                        State = ActionOptionState.WaitingForTileConfirm;

                        if (TileOccupier.GetTileOccupant(TargetTile) != null)
                        {
                            CombatUnit target = TileOccupier.GetTileOccupant(TargetTile).GetComponent<CombatUnit>();
                            if (target)
                            {
                                Prediction p = Menu.gameObject.AddComponent<Prediction>();
                                p.Effect = a.PredictedEffect(caster, target);
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
                    State = ActionOptionState.Inactive;
                    Menu.ShowAllMenus();
                    Engine.AssignControl(Menu);
                    Engine.TileManager.ClearHighlights(); //Engine.PathManager.ClearAll(false);
                    Engine.CameraManager.SetTargetPosition(caster.transform.position, .5f);
                    Engine.MapCursor.X = caster.gameObject.GetComponent<TileOccupier>().X;
                    Engine.MapCursor.Y = caster.gameObject.GetComponent<TileOccupier>().Y;
                    Engine.MapCursor.GoToTile();
                }
                break;
            case ActionOptionState.WaitingForTileConfirm:
                if (Engine.InputManager.Accept)
                {
                    Engine.InputManager.Accept = false;
                    Menu m = new GameObject().AddComponent<Menu>();
                    m.Attach = caster.gameObject;
                    m.Top = 180;
                    m.Left = Screen.width - 360;
                    m.name = "Confirm Effect";
                    m.ReturnControlTo = this;
                    Execute_MenuOption e = m.gameObject.AddComponent<Execute_MenuOption>();
                    e.Action = Action;
                    m.AddMenuOption("Execute", e);
                    NoExecute_MenuOption ne = m.gameObject.AddComponent<NoExecute_MenuOption>();
                    m.AddMenuOption("Quit", ne);
                }
                if (Engine.InputManager.Cancel)
                {
                    Engine.InputManager.Cancel = false;
                    State = ActionOptionState.WaitingForTileSelect;

                    Engine.TileManager.ClearHighlights();
                    a.HighlightSelectableTiles(caster);

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
        Engine.CombatManager.ActionTable[Action].HighlightSelectableTiles(Menu.Attach.GetComponent<CombatUnit>());
        Engine.AssignControl(this);
        State = ActionOptionState.WaitingForTileSelect;
        Menu.Attach.GetComponent<ActiveTurn>().ReturnControlTo = Menu;
        Engine.MapCursor.PanelMode = 2;
    }
}
