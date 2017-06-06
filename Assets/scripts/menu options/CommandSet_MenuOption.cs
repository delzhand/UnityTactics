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

        AvailableAction[] aas = Menu.Attach.GetComponents<AvailableAction>();
        foreach (AvailableAction a in aas)
        {
            if (a.Skillset == CommandSet)
            {
                Command_MenuOption cmo = m.gameObject.AddComponent<Command_MenuOption>();
                Type t = Type.GetType(a.Skillset);
                MethodInfo mi = t.GetMethod(a.Action + "_Name");
                String s = (string)mi.Invoke(null, null);
                cmo.Skillset = a.Skillset;
                cmo.Action = a.Action;
                m.AddMenuOption(s, cmo);
            }
        }

    }
}
