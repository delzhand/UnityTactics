using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPanel : MonoBehaviour {

    public int Mode;
    public int ModeOverride;
    public Camera Camera;

    public int Size = 100;

    private void Start()
    {
        Camera = GetComponentInChildren<Camera>();
        Camera.enabled = false;
    }

    private void Update()
    {
        TileOccupier tO = TileOccupier.GetTileOccupant(Engine.MapCursor.GetTile());
        if (tO != null && tO.gameObject == gameObject && Engine.MapCursor.PanelMode != 0)
        {
            Mode = Engine.MapCursor.PanelMode;
        }
        else
        {
            Mode = 0;
        }
    }

    private void OnGUI()
    {
        switch (Mode + ModeOverride)
        {
            case 1:
                Camera.enabled = true;
                LeftPicture();
                GUI.Label(new Rect(15, Screen.height - 32, 100, 26), gameObject.name);
                GUI.Label(new Rect(120, Screen.height - 100, 200, 75), GetComponent<CombatUnit>().GetVitals());
                GUI.Label(new Rect(120, Screen.height - 126, 200, 26), GetComponent<CombatUnit>().GetCombatInfo());
                break;
            case 2:
                Camera.enabled = true;
                RightPicture();
                GUI.Label(new Rect(Screen.width - 90, Screen.height - 32, 100, 26), gameObject.name);
                GUI.Label(new Rect(Screen.width - 250, Screen.height - 100, 200, 75), GetComponent<CombatUnit>().GetVitals());
                GUI.Label(new Rect(Screen.width - 250, Screen.height - 126, 200, 26), GetComponent<CombatUnit>().GetCombatInfo());
                break;
            default:
                Camera.enabled = false;
                break;
        }
    }

    private void LeftPicture()
    {
        float w = (float)Size / Screen.width;
        float h = (float)Size * 4 / 3 / Screen.height;
        float x = 10f / Screen.width;
        float y = 10f / Screen.height;
        Camera.rect = new Rect(x, y, w, h);
        Camera.transform.parent.localEulerAngles = new Vector3(0, 30, 0);
    }

    private void RightPicture()
    {
        float w = (float)Size / Screen.width;
        float h = (float)Size * 4 / 3 / Screen.height;
        float x = 1 - (10f / Screen.width) - w;
        float y = 10f / Screen.height;
        Camera.rect = new Rect(x, y, w, h);
        Camera.transform.parent.localEulerAngles = new Vector3(0, -30, 0);
    }

    public static void ClearAllPanels()
    {
        foreach(UnitPanel u in Engine.CombatManager.GetComponentsInChildren<UnitPanel>())
        {
            u.ModeOverride = 0;
        }
        Engine.MapCursor.PanelMode = 0;
    }

}
