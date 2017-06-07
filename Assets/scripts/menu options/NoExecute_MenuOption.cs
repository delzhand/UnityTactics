using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoExecute_MenuOption : MenuOption
{
    private void Start()
    {
        valid = true;
    }

    public override void SelectOption()
    {
        base.SelectOption();

        Action_MenuOption a_mo = (Action_MenuOption)Menu.ReturnControlTo;
        a_mo.StartTileSelection();

        Destroy(Menu.gameObject);
    }
}
