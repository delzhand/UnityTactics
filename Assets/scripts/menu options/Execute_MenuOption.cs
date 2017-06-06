using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Execute_MenuOption : MenuOption {

    public Action_MenuOption Command;

    private void Start()
    {
        valid = true;
    }

    public override void SelectOption()
    {
        base.SelectOption();
        Menu.Visible = false;
        Command.Execute(Engine.MapCursor.GetTile());
        Engine.MapCursor.X = Command.Menu.Attach.GetComponent<TileOccupier>().X;
        Engine.MapCursor.Y = Command.Menu.Attach.GetComponent<TileOccupier>().Y;
        Engine.MapCursor.GoToTile();
        Engine.MapCursor.Hide();
        UnitPanel.ClearAllPanels();
    }
}
