using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class Statics : MonoBehaviour
{

    public static List<Tile> AffectedTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(origin);
        return tiles;
    }

    public static List<Tile> AffectedTiles(int radius, int vTolerance, Tile origin, bool includeOrigin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile[] range = Engine.TileManager.FindTilesByRadius(origin, radius, includeOrigin);
        foreach (Tile t in range)
        {
            if (Mathf.Abs(origin.Height - t.Height) <= vTolerance)
            {
                tiles.Add(t);
            }
        }
        return tiles;
    }

    public static void HighlightTiles(List<Tile> tiles)
    {
        Material m = Resources.Load("graphics/materials/utility/yellow_highlight", typeof(Material)) as Material;
        foreach (Tile t in tiles)
        {
            t.Highlight(m);
        }
    }

    public static Tile[] RadiusTiles(Tile tile, int radius, bool includeOrigin)
    {
        return Engine.TileManager.FindTilesByRadius(tile, radius, includeOrigin);
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

    public static int Mod2(int xa, bool critical, Element element, CombatUnit caster, CombatUnit target, MethodInfo formula)
    {
        // Critical
        if (critical)
        {
            xa += UnityEngine.Random.Range(1, (xa - 1));
        }

        // Caster Elemental Strengthen

        // Caster Attack Up support

        // Caster Martial Arts

        // Caster Berserk

        // Target Defense Up

        // Target Protect

        // Target Charging

        // Target Sleeping

        // Target Chicken/Frog

        // Caster+Target Zodiac
        xa = ZodiacCompatibility.Modify(xa, caster, target);

        int damage = (int)formula.Invoke(null, new object[] { xa, caster });

        // Target Elemental Weak

        // Target Elemental Halved

        // Target Elemental Absorb

        return damage;
    }

    public static int Mod5(Element element, int spellPower, CombatUnit caster, CombatUnit target)
    {
        int damage = 0;
        float frac = 1;
        int xa = caster.MA;

        // Caster Elemental Strengthen

        // Caster Magic Up

        // Target Magic Defend Up

        // Target Shell

        // Caster+Target Zodiac
        xa = ZodiacCompatibility.Modify(xa, caster, target);

        // Caster/Target Faith status

        // Weather Elemental effect

        // Target Elemental Weakness

        // Target Elemental Half

        // Target Elemental Absorb

        damage = (int)(caster.Faith * target.Faith * spellPower * xa * frac / 10000f);

        return damage;
    }
}
