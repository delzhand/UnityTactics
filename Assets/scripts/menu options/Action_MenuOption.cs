using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public class Action_MenuOption : MenuOption
{

    public string Action;
    public bool Waiting = false;

    private void Start()
    {
        valid = true;
    }

    private void Update()
    {
        if (Waiting)
        {
            if (Engine.TimelineManager.Clear())
            {
                PostExecute();
            }
        }

        if (Engine.InputManager.Attach == this)
        {
            Engine.MapCursor.DelegatedInput();
            if (Engine.InputManager.Accept)
            {

                Engine.InputManager.Accept = false;
                Tile TargetTile = Engine.MapCursor.GetTile();
                if (TargetTile.CurrentlySelectable)
                {
                    Engine.InputManager.Accept = false;

                    TargetTile = Engine.MapCursor.GetTile();
                    Action a = Engine.CombatManager.ActionTable[Action];
                    CombatUnit caster = Menu.Attach.GetComponent<CombatUnit>();
                    a.HighlightAffectedTiles(TargetTile, caster);
                    caster.gameObject.GetComponent<UnitPanel>().ModeOverride = 1;


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

        }
    }

    public override void SelectOption()
    {
        base.SelectOption();
        Menu.HideAllMenus();
        Engine.CombatManager.ActionTable[Action].HighlightSelectableTiles(Menu.Attach.GetComponent<CombatUnit>());
        Engine.AssignControl(this);
        Menu.Attach.GetComponent<ActiveTurn>().ReturnControlTo = Menu;
        Engine.MapCursor.PanelMode = 2;
    }

    public virtual void Execute(Tile t)
    {
        Engine.TileManager.ClearHighlights();

        CombatUnit caster = Menu.Attach.GetComponent<CombatUnit>();

        if (Engine.CombatManager.ActionTable[Action].CTR == 0)
        {
            // Fast Action
            Engine.CombatManager.ActionTable[Action].Execute(caster, t, null);
            Waiting = true;
            Menu.Attach.GetComponent<ActiveTurn>().WaitState = WaitState.Resolving;
            Engine.AssignControl(Menu.Attach.GetComponent<ActiveTurn>());
        }
        else
        {
            //// Slow Action
            //SlowAction sa = Engine.CombatManager.gameObject.AddComponent<SlowAction>();
            //sa.Action = Action;
            //sa.Caster = Menu.Attach.GetComponent<CombatUnit>();
            //sa.TargetTile = t;
            //sa.Type = type;
            //sa.CTR = ctr;
            //sa.Caster.gameObject.AddComponent<Charging>();
            //PostExecute();
        }
    }

    public virtual void PostExecute()
    {
        Menu.Attach.GetComponent<ActiveTurn>().ActPoints = 0;
        Menu.Attach.GetComponent<ActiveTurn>().WaitState = WaitState.None;
        Menu.DestroyAllMenus();
        Engine.AssignControl(Menu.Attach.GetComponent<ActiveTurn>());
    }
}
