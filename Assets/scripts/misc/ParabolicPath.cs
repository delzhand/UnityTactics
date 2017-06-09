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

    public static int MinimumArc(Vector3 origin, Vector3 target, int maxArc, int resolution)
    {
        GameObject test = new GameObject("projectile");
        BoxCollider collider = test.AddComponent<BoxCollider>();
        collider.size = new Vector3(.2f, .2f, .2f);

        // Try each arc between 0 and the max
        for (int arc = 0; arc <= maxArc; arc++)
        {
            bool clear = true;

            // Test several points along arc
            for (int j = 0; j <= resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 position = Vector3.Lerp(origin, target, t);
                position.y += Mathf.Sin(Mathf.PI * t) * arc;
                collider.center = position;

                // If the position is within x distance of the origin or target, don't check collision
                if (Vector3.Distance(position, origin) < .5f || Vector3.Distance(position, target) < .5f)
                {
                    continue;
                }

                // Check collision against level geometry
                foreach (Transform transform in Engine.TileManager.transform)
                {
                    BoxCollider collider2 = transform.GetComponent<BoxCollider>();
                    if (collider.bounds.Intersects(collider2.bounds))
                    {
                        clear = false;
                        continue;
                    }
                }

                // Check collision against units
                if (clear)
                {
                    foreach (Transform transform in Engine.CombatManager.transform)
                    {
                        BoxCollider collider2 = transform.GetComponent<BoxCollider>();
                        if (collider.bounds.Intersects(collider2.bounds))
                        {
                            clear = false;
                            continue;
                        }
                    }
                }

            }

            if (clear)
            {
                Destroy(test);
                return arc;
            }
        }

        Destroy(test);
        return -1;
    }

    // Returns a pair. Pair.first is the unit hit or null (for level geometry), Pair.second is when the hit occurs (between 0 and 1)
    public static Pair<CombatUnit, float> ProjectileInterrupt(Vector3 origin, Vector3 target, float arc, int resolution)
    {
        GameObject test = new GameObject("projectile");
        BoxCollider collider = test.AddComponent<BoxCollider>();
        collider.size = new Vector3(.2f, .2f, .2f);

        // Test several points along arc
        for (int j = 0; j <= resolution; j++)
        {
            float t = j / (float)resolution;
            Vector3 position = Vector3.Lerp(origin, target, t);
            position.y += Mathf.Sin(Mathf.PI * t) * arc;
            collider.center = position;

            // If the position is within x distance of the origin
            if (Vector3.Distance(position, origin) < .5f)
            {
                continue;
            }

            foreach (Transform transform in Engine.TileManager.transform)
            {
                BoxCollider collider2 = transform.GetComponent<BoxCollider>();
                if (collider.bounds.Intersects(collider2.bounds))
                {
                    Destroy(test);
                    return new Pair<CombatUnit, float>(null, t);
                }
            }
            foreach (Transform transform in Engine.CombatManager.transform)
            {
                BoxCollider collider2 = transform.GetComponent<BoxCollider>();
                if (collider.bounds.Intersects(collider2.bounds))
                {
                    Destroy(test);
                    return new Pair<CombatUnit, float>(transform.gameObject.GetComponent<CombatUnit>(), t);
                }
            }
        }

        Destroy(test);
        return new Pair<CombatUnit, float>(null, 1);
    }
}