using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour {

    public string BoxMessage;
    public string BoxMessageLabel;

    public float MaxLife = 0;
    private float Timer = 0;

    public MonoBehaviour ReturnControlTo;

    private void OnGUI()
    {
        int top = 95;
        Vector2 size = Engine.GuiStyleBox.CalcSize(new GUIContent(BoxMessage));
        int left = Screen.width/2 - (int)Math.Round(size.x / 2f);
        GUI.Box(new Rect(left, top + 10, size.x, 25), BoxMessage, Engine.GuiStyleBox);
        GUI.Label(new Rect(left+4, top, 100, 20), BoxMessageLabel, Engine.GuiStyleLabel);
    }

    private void Update()
    {
        if (MaxLife > 0)
        {
            Timer += Time.deltaTime;
            if (Timer > MaxLife)
            {
                Done();
            }
        }

        if (Engine.InputManager.Attach == this)
        {
            if (Engine.InputManager.Accept || Engine.InputManager.Cancel)
            {
                Engine.InputManager.Accept = false;
                Engine.InputManager.Cancel = false;
                Done();
            }
        }
    }

    private void Done()
    {
        if (ReturnControlTo != null)
        {
            Engine.AssignControl(ReturnControlTo);
        }
        Destroy(gameObject);
    }

    public static Message CreateNew(string label, string text, float maxLife)
    {
        Message m = new GameObject("Message").AddComponent<Message>();
        m.BoxMessage = text;
        m.BoxMessageLabel = label;
        m.MaxLife = maxLife;
        return m;
    }
}
