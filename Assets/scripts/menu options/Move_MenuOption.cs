using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_MenuOption : MenuOption {

	// Update is called once per frame
	void Update () {
        valid = Menu.Attach.GetComponent<ActiveTurn>().MovePoints > 0;
        if (SettingsManager.FreeMove)
        {
            valid = true;
        }
    }

    public override void SelectOption()
    {
        base.SelectOption();
        Menu.Visible = false;
        Menu.Attach.GetComponent<Mover>().ClearWaypoints();
        Menu.Attach.GetComponent<Mover>().ShowMoveTiles();
        Engine.AssignControl(Menu.Attach.GetComponent<ActiveTurn>());
        Menu.Attach.GetComponent<ActiveTurn>().WaitState = WaitState.WaitingForMovement;
        Menu.Attach.GetComponent<ActiveTurn>().ReturnControlTo = Menu;
    }

    public override void BadSelection()
    {
        Debug.LogError("Unit cannot move.");
    }
}
