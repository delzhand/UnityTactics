using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTextItem : MonoBehaviour {
    public MenuOption menuOption;

    private static readonly Color inactiveColor = new Color(99 / 255f, 90 / 255f, 74 / 255f);
    private static readonly Color inactiveShadow = new Color(139 / 255f, 131 / 255f, 114 / 255f);
    private static readonly Color activeColor = new Color(49 / 255f, 41 / 255f, 33 / 255f);
    private static readonly Color activeShadow = new Color(133 / 255f, 124 / 255f, 108 / 255f);

    private TextMesh text;
    private TextMesh shadow;

    private void Update()
    {
        if (text && shadow)
        {
            text.GetComponent<Renderer>().material.color = menuOption.valid ? activeColor : inactiveColor;
            shadow.GetComponent<Renderer>().material.color = menuOption.valid ? activeShadow : inactiveShadow;
        }
    }

    public void SetOption(MenuOption option)
    {
        menuOption = option;
        text = transform.Find("Text").GetComponent<TextMesh>();
        text.text = option.text;
        shadow = transform.Find("Text Shadow").GetComponent<TextMesh>();
        shadow.text = option.text;
    }
}
