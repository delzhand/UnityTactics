using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prediction : MonoBehaviour {

    public string Effect;

    private void OnGUI()
    {
        GUI.Label(new Rect((Screen.width - 140)/2, Screen.height - 50, 140, 30), Effect);
    }
}
