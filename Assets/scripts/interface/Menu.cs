using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Menu : MonoBehaviour {

    public MonoBehaviour ReturnControlTo;

    private string output;
    public int SelectedIndex;

    public float Left = 0;
    public float Top = 0;
    public float Z = 0;

    private int width = 0;

    public GameObject Attach;

    public bool Visible = true;

    public static int StackCount = 0;

    void OnGUI()
    {
        //if (width == 0)
        //{
        //    updateWidth();
        //}
        //if (Visible)
        //{
        //    GUI.Box(new Rect(Left, Top + 10, width, 20 + GetComponents<MenuOption>().Length * 15), output, Engine.GuiStyleBox);
        //    GUI.Label(new Rect(Left + 4, Top, 100, 20), "Menu", Engine.GuiStyleLabel);
        //}
    }

    // Use this for initialization
    void Start() {
        Engine.AssignControl(this);
        tag = "Menus";
        GenerateRenderObjects();
        Menu.StackCount++;
    }

    // Update is called once per frame
    void Update() {
        Transform cursor = transform.Find("Menu Container/Cursor").transform;

        if (Visible)
        {
            transform.Find("Menu Container").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Menu Container").gameObject.SetActive(false);
        }

        if (Engine.InputManager.Attach != this)
        {
            cursor.transform.Find("Finger").GetComponent<Bounce>().enabled = false;
        }
        else if (Engine.InputManager.Attach == this)
        {
            UpdateMenuPosition();
            cursor.localPosition = new Vector3(cursor.localPosition.x, -3 - (SelectedIndex * 5), cursor.localPosition.z);
            cursor.transform.Find("Finger").GetComponent<Bounce>().enabled = true;

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

    public void GenerateRenderObjects()
    {
        Z = 1 + Menu.StackCount;
        GameObject container = (GameObject)Instantiate(Resources.Load("prefabs/gui/MenuContainer"));
        container.name = "Menu Container";
        container.transform.parent = transform;
        container.transform.localEulerAngles = Vector3.zero;
        MenuOption[] options = GetComponents<MenuOption>();

        Vector2 size = new Vector2(0, 0);

        for (int i = 0; i < options.Length; i++)
        {
            GameObject item = (GameObject)Instantiate(Resources.Load("prefabs/gui/MenuTextItem"));
            item.transform.parent = container.transform;
            MenuTextItem textItem = item.GetComponent<MenuTextItem>();
            textItem.SetOption(options[i]);
            item.transform.localPosition = new Vector3(0, -5 * i, -.15f);
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localScale = Vector3.one;
            size.y += 5;
            size.x = Mathf.Max(size.x, textItem.transform.Find("Text").GetComponent<Renderer>().bounds.extents.x * 20);
        }
        Vector2 padding = new Vector2(3, 3);
        size += padding;
        container.transform.Find("Background").transform.localScale = new Vector3(size.x, size.y, .15f);
        container.transform.Find("Background").transform.localPosition = new Vector3((size.x - padding.x)/ 2, -(size.y - padding.y) / 2, 0);
        GameObject cursor = (GameObject)Instantiate(Resources.Load("prefabs/gui/MenuCursor"));
        cursor.transform.parent = container.transform;
        cursor.transform.localPosition = new Vector3(-5.5f, -3f, -1f);
        cursor.transform.localScale = new Vector3(1f, 1f, 1f);
        cursor.name = "Cursor";
        container.SetLayer(LayerMask.NameToLayer("UI"), true);
    }

    public void UpdateMenuPosition()
    {
        UIScale uiScale = GameObject.Find("UI Camera").GetComponent<UIScale>();
        Vector3 uiCenter = uiScale.gameObject.transform.position;
        transform.Find("Menu Container").transform.position = uiCenter + new Vector3((Left - .5f) * uiScale.xScale, (Top - .5f) * -uiScale.yScale, 10 - Z);
    }

    private void OnDestroy()
    {
        MenuOption[] options = GetComponents<MenuOption>();
        for (int i = 0; i < GetComponents<MenuOption>().Length; i++)
        {
            Destroy(options[i]);
        }
        Menu.StackCount--;
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
