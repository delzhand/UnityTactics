using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicPath : MonoBehaviour
{
    public Vector3 Origin;
    public Vector3 Target;
    public float Arc;
    public float StopAt;

    public GameObject Projectile;
    public float FlightDuration;
    public float Timer;
    public Vector3 LastPosition;

    //private void OnGUI()
    //{
    //    int resolution = 30;
    //    for(int i = 0; i <= resolution; i++)
    //    {
    //        Vector3 pos = positionAtTime(i / (float)resolution);
    //        Vector2 screenPos = Engine.CameraManager.Camera.WorldToScreenPoint(pos);

    //        GUIStyle g = new GUIStyle(Engine.GuiStyleLabel);
    //        g.fontSize = 20;
    //        g.alignment = TextAnchor.MiddleCenter;
    //        int size = 30;
    //        GUI.Label(new Rect(screenPos.x - size/2, Screen.height - screenPos.y + size/2, size, size), "·", g);
    //    }
    //}

    private void Update()
    {
        Timer = Mathf.Min(Timer + Time.deltaTime, FlightDuration);
        float t = Timer / FlightDuration;

        Projectile.transform.position = positionAtTime(t);

        Vector3 velocity = Projectile.transform.position - LastPosition;
        if (velocity != Vector3.zero)
        {
            Projectile.transform.rotation = Quaternion.LookRotation(velocity);
        }

        if (Projectile != null)
        {
            LastPosition = Projectile.transform.position;
        }

        if (Timer >= FlightDuration * StopAt)
        {
            Destroy(gameObject);
        }
    }

    private Vector3 positionAtTime(float t)
    {
        Vector3 position = Vector3.Lerp(Origin, Target, t);
        position.y += Mathf.Sin(Mathf.PI * t) * Arc;
        return position;
    }

}