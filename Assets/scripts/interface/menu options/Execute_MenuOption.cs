using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Execute_MenuOption : MenuOption {

    public string Action;
    public bool WaitingForExecution;

    private void Start()
    {
        valid = true;
    }

    private void Update()
    {
        if (WaitingForExecution && Engine.TimelineManager.Clear())
        {
            PostExecute();
        }
    }

    public override void SelectOption()
    {
        base.SelectOption();
        Menu.HideAllMenus();

        UnitPanel.ClearAllPanels();
        Tile t = Engine.MapCursor.GetTile();
        Engine.MapCursor.X = Menu.Attach.GetComponent<TileOccupier>().X;
        Engine.MapCursor.Y = Menu.Attach.GetComponent<TileOccupier>().Y;
        Engine.MapCursor.GoToTile();
        Engine.MapCursor.Hide();

        Engine.TileManager.ClearHighlights();

        CombatUnit caster = Menu.Attach.GetComponent<CombatUnit>();

        if (Engine.CombatManager.ActionTable[Action].CTR == 0)
        {
            // Fast Action
            Engine.CombatManager.ActionTable[Action].Execute(caster, t, null);
            WaitingForExecution = true;
            Engine.AssignControl(this);
        }
        else
        {
            // Slow Action
            SlowAction sa = Engine.CombatManager.gameObject.AddComponent<SlowAction>();
            sa.Action = Action;
            sa.Caster = Menu.Attach.GetComponent<CombatUnit>();
            sa.TargetTile = t;
            sa.Action = Action;
            sa.CTR = Engine.CombatManager.ActionTable[Action].CTR;
            sa.Caster.gameObject.AddComponent<Charging>();
            PostExecute();
        }

    }

    public void PostExecute()
    {
        Menu.Attach.GetComponent<ActiveTurn>().ActPoints = 0;
        Menu.Attach.GetComponent<ActiveTurn>().WaitState = WaitState.None;
        Menu.DestroyAllMenus();
        Engine.AssignControl(Menu.Attach.GetComponent<ActiveTurn>());
    }
}
