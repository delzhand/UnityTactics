using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CommandSet_MenuOption : MenuOption {

    public string CommandSet;

    private void Start()
    {
        valid = true;
    }

    public override void SelectOption()
    {
        base.SelectOption();

        Menu m = new GameObject().AddComponent<Menu>();
        m.Attach = Menu.Attach;
        m.name = Menu.Attach.name + " (" + CommandSet + ")";
        m.ReturnControlTo = Menu;
        m.Top = 30;
        m.Left = 230;

        string[] actions = Menu.Attach.GetComponent<AvailableActions>().Actions;
        foreach (string s in actions)
        {
            Action_MenuOption a_mo = m.gameObject.AddComponent<Action_MenuOption>();
            a_mo.Action = s;
            Action a = Engine.CombatManager.ActionTable[s];
            m.AddMenuOption(a.GetName(), a_mo);
        }
    }
}
