using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour {

    public static InputManager InputManager;
    public static CameraManager CameraManager;
    public static PathManager PathManager;
    public static TileManager TileManager;
    public static CombatManager CombatManager;
    public static SoundManager SoundManager;
    public static TimelineManager TimelineManager;

    public static MapCursor MapCursor;

    public static GUIStyle GuiStyleLabel;
    public static GUIStyle GuiStyleBox;

    private void OnGUI()
    {
        Engine.GuiStyleLabel = new GUIStyle(GUI.skin.label);
        Engine.GuiStyleLabel.fontSize = 10;
        Engine.GuiStyleLabel.alignment = TextAnchor.UpperLeft;
        Engine.GuiStyleBox = new GUIStyle(GUI.skin.box);
        Engine.GuiStyleBox.fontSize = 14;
        Engine.GuiStyleBox.alignment = TextAnchor.UpperLeft;
    }

    void Awake ()
    {
        Engine.InputManager = GetComponent<InputManager>();
        Engine.CameraManager = GameObject.Find("Camera Rig").GetComponent<CameraManager>();
        Engine.MapCursor = GameObject.Find("Map Cursor").GetComponent<MapCursor>();
        Engine.PathManager = GameObject.Find("Tiles").GetComponent<PathManager>();
        Engine.TileManager = GameObject.Find("Tiles").GetComponent<TileManager>();
        Engine.CombatManager = GameObject.Find("Units").GetComponent<CombatManager>();
        Engine.SoundManager = GetComponent<SoundManager>();
        Engine.TimelineManager = GameObject.Find("Timelines").GetComponent<TimelineManager>();
    }

    public static void AssignControl(MonoBehaviour component)
    {
        //Debug.Log("Control assigned to " + (component != null ? component.name : "null"));
        Engine.InputManager.Attach = component;
    }
}
