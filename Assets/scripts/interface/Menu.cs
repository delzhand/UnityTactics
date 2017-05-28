using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Menu : MonoBehaviour {

    public MonoBehaviour ReturnControlTo;

    private string output;
    public int SelectedIndex;

    public int Left = 10;
    public int Top = 10;

    private int width = 0;

    public GameObject Attach;

    public bool Visible = true;

    void OnGUI()
    {
        if (width == 0)
        {
            updateWidth();
        }
        if (Visible)
        {
            GUI.Box(new Rect(Left, Top + 10, width, 20 + GetComponents<MenuOption>().Length * 15), output, Engine.GuiStyleBox);
            GUI.Label(new Rect(Left + 4, Top, 100, 20), "Menu", Engine.GuiStyleLabel);
        }
    }

    // Use this for initialization
    void Start() {
        Engine.AssignControl(this);
        tag = "Menus";
    }

    // Update is called once per frame
    void Update() {
        if (Engine.InputManager.Attach == this)
        {
            int itemCount = GetComponents<MenuOption>().Length;
            if (Engine.InputManager.Down())
            {
                SelectedIndex = (SelectedIndex + 1) % itemCount;
                Engine.SoundManager.PlaySound("cursor_change");
            }
            else if (Engine.InputManager.Up())
            {
                SelectedIndex = (SelectedIndex + itemCount - 1) % itemCount;
                Engine.SoundManager.PlaySound("cursor_change");
            }
            else if (Engine.InputManager.Accept)
            {
                Engine.InputManager.Accept = false;
                MenuOption[] options = GetComponents<MenuOption>();
                if (options[SelectedIndex].valid)
                {
                    options[SelectedIndex].SelectOption();
                    Engine.SoundManager.PlaySound("cursor_select");
                }
                else
                {
                    options[SelectedIndex].BadSelection();
                    Engine.SoundManager.PlaySound("cursor_invalid");
                }
            }
            else if (Engine.InputManager.Cancel)
            {
                Engine.InputManager.Cancel = false;
                Engine.AssignControl(ReturnControlTo);
                Engine.SoundManager.PlaySound("cursor_cancel");
                Destroy(this);
                Destroy(gameObject);
            }
            updateOutput();
        }
    }

    private void OnDestroy()
    {
        MenuOption[] options = GetComponents<MenuOption>();
        for (int i = 0; i < GetComponents<MenuOption>().Length; i++)
        {
            Destroy(options[i]);
        }
    }

    private void updateWidth()
    {
        MenuOption[] options = GetComponents<MenuOption>();
        for (int i = 0; i < GetComponents<MenuOption>().Length; i++)
        {
            Vector2 size = Engine.GuiStyleBox.CalcSize(new GUIContent("> " + options[i].text));
            width = (int)Math.Max(size.x, width);
        }
    }

    private void updateOutput()
    {
        StringBuilder sb = new StringBuilder();
        MenuOption[] options = GetComponents<MenuOption>();
        for (int i = 0; i < GetComponents<MenuOption>().Length; i++)
        {
            string marker = "  ";
            if (Engine.InputManager.Attach == this && options[i].valid && i == SelectedIndex)
            {
                marker = ">";
            }
            else if (Engine.InputManager.Attach == this && i == SelectedIndex)
            {
                marker = "X";
            }
            sb.AppendLine(marker + options[i].text);
        }
        output = sb.ToString();

    }

    public void AddMenuOption(string text)
    {
        MenuOption m = gameObject.AddComponent<MenuOption>();
        m.Menu = this;
        m.text = text;

    }

    public void AddMenuOption(string text, MenuOption option)
    {
        option.text = text;
        option.Menu = this;
    }

    public MenuOption GetSelected()
    {
        return GetComponents<MenuOption>()[SelectedIndex];
    }

    public static void ShowAllMenus()
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("Menus");
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].GetComponent<Menu>().Visible = true;
        }
    }


    public static void HideAllMenus()
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("Menus");
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].GetComponent<Menu>().Visible = false;
        }
    }

        public static void DestroyAllMenus()
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("Menus");
        for (int i = 0; i < menus.Length; i++)
        {
            //Debug.Log("Destroying menu " + menus[i].name);
            Destroy(menus[i]);
        }
    }
}
