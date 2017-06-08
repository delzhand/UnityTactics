using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Act_MenuOption : MenuOption {

	void Update () {
        valid = Menu.Attach.GetComponent<ActiveTurn>().ActPoints > 0;
        if (SettingsManager.FreeAct)
        {
            valid = true;
        }
    }

    public override void SelectOption()
    {
        base.SelectOption();
        Menu m = new GameObject().AddComponent<Menu>();
        m.name = Menu.Attach.name + " Action";
        m.Top = 30;
        m.Left = 120;
        m.ReturnControlTo = Menu;
        m.Attach = Menu.Attach;
        Engine.InputManager.Attach = m;

        // Attack
        Attack_MenuOption a_mo = m.gameObject.AddComponent<Attack_MenuOption>();
        m.AddMenuOption("Attack", a_mo);

        // Command Sets
        List<string> commandSets = new List<string>();
        string[] actions = Menu.Attach.GetComponent<AvailableActions>().Actions;
        foreach(string s in actions)
        {
            if (!Engine.CombatManager.ActionTable.ContainsKey(s))
            {
                throw new Exception(s + " not in Action Table");
            }
            Action a = Engine.CombatManager.ActionTable[s];
            if (!commandSets.Contains(a.CommandSet))
            {
                commandSets.Add(a.CommandSet);
            }
        }
        foreach(string s in commandSets)
        {
            CommandSet_MenuOption cs_mo = m.gameObject.AddComponent<CommandSet_MenuOption>();
            cs_mo.CommandSet = s;
            m.AddMenuOption(Engine.CombatManager.CommandSetTable[s].GetName(), cs_mo);
        }
    }

    public override void BadSelection()
    {
        Debug.Log("Unit cannot act.");
    }
}
