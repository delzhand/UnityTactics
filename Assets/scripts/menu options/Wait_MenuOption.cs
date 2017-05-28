using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait_MenuOption : MenuOption {

	void Start () {
        valid = true;
	}
	
    public override void SelectOption()
    {
        base.SelectOption();
        Menu.Visible = false;
        GameObject facing_selector = Instantiate(Resources.Load("prefabs/units/Facing Selector")) as GameObject;
        facing_selector.name = "Facing Selector";
        facing_selector.transform.parent = Menu.Attach.transform;
        facing_selector.transform.localPosition = Vector3.zero;
        facing_selector.transform.localRotation = Quaternion.identity;
        Engine.MapCursor.Hide();

        if (SettingsManager.ShowMoveConfirmMessage)
        {
            Message m = Menu.Attach.AddComponent<Message>();
            m.ReturnControlTo = Menu.Attach.GetComponent<Facer>();
            m.BoxMessage = "Choose a direction with the direction buttons.";
            m.BoxMessageLabel = "Check";
            Engine.InputManager.Attach = m;
        }
        else
        {
            Engine.InputManager.Attach = Menu.Attach.GetComponent<Facer>();
        }
        Menu.DestroyAllMenus();
    }
}
