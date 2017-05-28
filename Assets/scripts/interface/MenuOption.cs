using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuOption : MonoBehaviour {

    public string text;
    public bool valid;
    public Menu Menu;

    private void Update()
    {
        
    }

    public virtual void SelectOption()
    {
        Engine.InputManager.Accept = false;
    }

    public virtual void BadSelection()
    {
        Debug.LogError("Menu option \"" + text + "\" added via legacy method and will not work.");
    }
}
