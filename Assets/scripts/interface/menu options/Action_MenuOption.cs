using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;


public class Action_MenuOption : TargetableAction_MenuOption
{

    public string Action;

    private void Start()
    {
        Action a = Engine.CombatManager.ActionTable[Action];
        Caster = Menu.Attach.GetComponent<CombatUnit>();
        valid = a.CanBeCast(Caster);
    }

    public override void HighlightSelectableTiles()
    {
        Engine.CombatManager.ActionTable[Action].HighlightSelectableTiles(Caster);
    }

    public override void HighlightAffectedTiles(Tile TargetTile)
    {
        Engine.CombatManager.ActionTable[Action].HighlightAffectedTiles(TargetTile, Caster);
    }

    public override void SetExecute(Execute_MenuOption e)
    {
        e.Action = Action;
    }
}
