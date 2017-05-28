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

        AvailableAction[] aas = Menu.Attach.GetComponents<AvailableAction>();
        foreach(AvailableAction a in aas)
        {
            if (a.Skillset == "Attack")
            {
                Command_MenuOption cmo = m.gameObject.AddComponent<Command_MenuOption>();
                Type t = Type.GetType(a.Skillset);
                MethodInfo mi = t.GetMethod("GetName");
                String s = (string)mi.Invoke(null, null);
                cmo.Skillset = a.Skillset;
                cmo.Action = a.Action;
                m.AddMenuOption(s, cmo);
            }
            else
            {
                Skillset_MenuOption smo = m.gameObject.AddComponent<Skillset_MenuOption>();
                smo.Skillset = a.Skillset;
                Type t = Type.GetType(a.Skillset);
                MethodInfo mi = t.GetMethod("GetName");
                String s = (string)mi.Invoke(null, null);
                m.AddMenuOption(s, smo);
            }
        }
    }

    public override void BadSelection()
    {
        Debug.Log("Unit cannot act.");
    }
}
