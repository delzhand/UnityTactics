using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingText : MonoBehaviour {

    public string Text;
    public float Delay;
    public float Duration;
    public Vector2 Offset;
    public float timer;

	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
	}

    private void OnGUI()
    {
        if (timer >= Delay && timer <= Delay + Duration)
        {
            GUIStyle g = Engine.GuiStyleLabel;
            g.fontSize = 20;
            g.alignment = TextAnchor.UpperCenter;
            Vector2 v = UpdatePosition();
            GUI.Label(new Rect(v.x, Screen.height - v.y, 30, 30), Text, g);
        }
        else if (timer > Delay + Duration)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        FlyingText[] f = GetComponents<FlyingText>();
        if (f.Length == 1)
        {
            Destroy(gameObject);
        }
    }

    public virtual Vector2 UpdatePosition()
    {
        Vector3 screenOrigin = Engine.CameraManager.Camera.WorldToScreenPoint(transform.position);
        screenOrigin.x += Offset.x;
        screenOrigin.y += Offset.y;
        return new Vector2(screenOrigin.x, screenOrigin.y);
    }

    public static void CreateFromCharacters(string text, MonoBehaviour m, float delay)
    {
        Vector3 v = m.transform.position;
        v += new Vector3(0, 1f, 0);
        FlyingText.CreateFromCharacters(text, v, delay);
    }

    public static void CreateFromCharacters(string text, Vector3 position, float delay)
    {
        FlyingText.CreateFromCharacters(text, position, 1.5f, delay);
    }

    public static void CreateFromCharacters(string text, Vector3 position, float duration, float delay)
    {
        float letterWidth = 12;
        float letterDelay = .025f;
        GameObject g = new GameObject("Flying Text");
        g.transform.position = position;
        for (int i = 0; i<text.Length; i++)
        {
            FlyingText f = g.AddComponent<FlyingText>();
            float linePosition = i * letterWidth;
            linePosition -= (text.Length * letterWidth / 2f);
            linePosition -= letterWidth / 2f;
            f.Offset = new Vector2(linePosition, 30);
            f.Delay = delay + (i * letterDelay);
            f.Duration = duration - (i * letterDelay);
            f.Text = text.Substring(i, 1);
        }
    }
}
