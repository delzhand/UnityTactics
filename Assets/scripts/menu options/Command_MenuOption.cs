using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public class Command_MenuOption : MenuOption
{

    public Tile TargetTile;
    public string Skillset;
    public string Action;
    public bool Waiting = false;
    //public MethodInfo Effect;
    //public MethodInfo HighlightTarget;

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
    }

    public override void SelectOption()
    {
        base.SelectOption();
        Menu.HideAllMenus();

        // Set allowed tiles
        Material m = Resources.Load("graphics/materials/utility/red_highlight", typeof(Material)) as Material;
        Type t = Type.GetType(Skillset);
        MethodInfo mi = t.GetMethod(Action + "_SelectableRange");
        object[] parameters = new object[] { Menu.Attach.GetComponent<CombatUnit>() };
        Tile[] tiles = (Tile[])mi.Invoke(new Attack(), parameters);
        foreach (Tile tile in tiles)
        {
            tile.Highlight(m);
            tile.CurrentlySelectable = true;
        }

        Engine.AssignControl(Menu.Attach.GetComponent<ActiveTurn>());
        Menu.Attach.GetComponent<ActiveTurn>().WaitState = WaitState.WaitingForAction;
        Menu.Attach.GetComponent<ActiveTurn>().ReturnControlTo = Menu;
        Engine.MapCursor.PanelMode = 2;

        //Effect = t.GetMethod(Action + "_PredictedEffect");
        //HighlightTarget = t.GetMethod(Action + "_HighlightAffectedTiles");
    }

    public virtual void Execute(Tile t)
    {
        Engine.TileManager.ClearHighlights();

        Type type = Type.GetType(Skillset);
        int ctr = (int)type.GetMethod(Action + "_CTR").Invoke(null, null);
        CombatUnit caster = Menu.Attach.GetComponent<CombatUnit>();
        

        if (ctr == 0)
        {
            // Fast Action
            type.GetMethod(Action + "_Execute").Invoke(null, new object[] { caster, t, null });
            Waiting = true;
            Menu.Attach.GetComponent<ActiveTurn>().WaitState = WaitState.Resolving;
            Engine.AssignControl(Menu.Attach.GetComponent<ActiveTurn>());
        }
        else
        {
            // Slow Action
            SlowAction sa = Engine.CombatManager.gameObject.AddComponent<SlowAction>();
            sa.Action = Action;
            sa.Caster = Menu.Attach.GetComponent<CombatUnit>();
            sa.TargetTile = t;
            sa.Type = type;
            sa.CTR = ctr;
            sa.Caster.gameObject.AddComponent<Charging>();
            PostExecute();
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
