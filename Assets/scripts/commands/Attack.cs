using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Attack {

    public static string GetName()
    {
        return "Attack";
    }

    #region Strike
    /**
     * Bare Hands, Knife, Ninja Sword, Sword, Knight Sword, Katana,
     * Staff, Rod, Flail, Axe, and Bag are considered "striking" weapons
     * and have a range of 1v3 from above and 1v2 from below.
     */
    public static string Strike_Name()
    {
        return SettingsManager.PSXTranslation ? "Attack" : "Attack";
    }

    public static Tile[] Strike_SelectableRange(CombatUnit Caster)
    {
        List<Tile> tileList = new List<Tile>();

        Tile origin = Caster.GetComponent<TileOccupier>().GetOccupiedTile();
        Direction[] checkDirections = new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West };
        for (int i = 0; i < checkDirections.Length; i++)
        {
            Direction d = checkDirections[i]; // direction to check
            Direction di = checkDirections[(i + 2) % 4]; // direction inverse for edge

            Tile[] tiles = Engine.TileManager.FindTilesInDirection(origin, d, 1);
            foreach (Tile t in tiles)
            {
                if (t != null)
                {
                    float difference = origin.Height - t.HeightAtEdge(di);
                    if (difference <= 3 && difference >= -2)
                    {
                        tileList.Add(t);
                    }
                }
            }
        }
        return tileList.ToArray();
    }

    public static void Strike_HighlightSelectableRange(CombatUnit caster)
    {
        Engine.TileManager.ClearHighlights();
        Tile[] tiles = Strike_SelectableRange(caster);
        foreach (Tile t in tiles)
        {
            t.Highlight("red_highlight");
        }
    }

    public static int Strike_CTR()
    {
        return 0;
    }

    public static string Strike_PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        return "-" + Strike_PredictedDamage(caster, target).ToString() + "HP " + Strike_PredictedSuccess(caster, target).ToString() + "%";
    }

    private static int Strike_PredictedDamage(CombatUnit caster, CombatUnit target)
    {
        MethodInfo formula = Type.GetType("Attack").GetMethod("Strike_Formula");
        int damage = Statics.Mod2(caster.PA, false, Element.None, caster, target, formula);
        return damage;
    }

    public static int Strike_Formula(int xa, CombatUnit caster)
    {
        return xa * caster.WP();
    }

    private static int Strike_PredictedSuccess(CombatUnit caster, CombatUnit target)
    {
        int baseHit = 100;
        Side side = CombatUnit.actionAngle(caster, target);
        int hitTarget = target.EvadeRate(side, baseHit);
        return 100 - hitTarget;
    }

    public static void Strike_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Strike Timeline").AddComponent<Timeline>();
        t.AdvanceTo(.1f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "sword_swing");
        t.Advance(.25f); // Moment of impact

        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        if (tO != null)
        {
            // TODO - Check if target is already dead

            targetUnit = tO.GetComponent<CombatUnit>();
            bool critical = (UnityEngine.Random.Range(1, 100) <= 5);
            MethodInfo formula = Type.GetType("Attack").GetMethod("Strike_Formula");
            int damage = Statics.Mod2(caster.PA, critical, Element.None, caster, targetUnit, formula);

            int hit = UnityEngine.Random.Range(1, 100);
            int baseHit = 100;
            Side side = CombatUnit.actionAngle(caster, targetUnit);
            int hitTarget = targetUnit.EvadeRate(side, baseHit);

            if (hit > hitTarget)
            {
                // Successful hit
                targetUnit.TakeDamage(damage);

                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0f, "Armature|Hurt", targetUnit.GetComponentInChildren<Animator>());
                t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
                t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .2f, "PhysicalDamage", targetTile);
                t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, damage.ToString(), targetUnit);
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
                t.Advance(.55f);
            }
            else
            {
                int evadeType = targetUnit.EvadeType(side);
                float targetFacing = tO.transform.eulerAngles.y;
                switch (evadeType)
                {
                    case 0:
                        // miss
                        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, tO.GetComponent<Facer>(), caster.GetComponent<TileOccupier>().GetOccupiedTile());
                        t.gameObject.AddComponent<Shimmy_TimelineEvent>().Init(t, .05f, tO.gameObject);
                        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "MISS", tO);
                        t.gameObject.AddComponent<FaceAngle_TimelineEvent>().Init(t, .55f, tO.GetComponent<Facer>(), targetFacing);
                        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
                        t.Advance(.55f);
                        break;
                    case 1:
                        // shield
                        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, tO.GetComponent<Facer>(), caster.GetComponent<TileOccupier>().GetOccupiedTile());
                        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|ShieldBlock", tO.GetComponentInChildren<Animator>());
                        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttackBlocked", caster.GetComponentInChildren<Animator>());
                        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "shield_block");
                        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "BLOCK", tO);
                        t.gameObject.AddComponent<FaceAngle_TimelineEvent>().Init(t, .55f, tO.GetComponent<Facer>(), targetFacing);
                        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
                        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .75f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
                        t.Advance(.75f);
                        break;
                    case 2:
                        // weapon
                        Debug.Log("Weapon Evasion timeline not implemented yet.");
                        break;
                }
            }

            // TODO - provoke post-action reactions
        }
        else
        {
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        }

        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }

    public static void Strike_HighlightAffectedTiles(Tile origin)
    {
        Statics.HighlightTiles(Strike_AffectedTiles(origin));
    }

    public static List<Tile> Strike_AffectedTiles(Tile origin)
    {
        return Statics.AffectedTiles(origin);
    }
    #endregion

    #region Bow
    /**
     * Bows require LOS or concave downward path
     */
    public static string Bow_Name()
    {
        return SettingsManager.PSXTranslation ? "Attack" : "Attack";
    }

    public static Tile[] Bow_SelectableRange(CombatUnit caster)
    {
        Tile origin = caster.GetComponent<TileOccupier>().GetOccupiedTile();

        List<Tile> tileList = new List<Tile>();
        List<Tile> maxRangeTiles = new List<Tile>(Statics.RadiusTiles(origin, 5, false));
        List<Tile> minRangeTiles = new List<Tile>(Statics.RadiusTiles(origin, 2, false));

        foreach (Tile t in maxRangeTiles)
        {
            if (!minRangeTiles.Contains(t))
            {
                tileList.Add(t);
            }
        }

        // HD = (shooter's height - target's height)
        // bow range = 5 + [HD / 2]
        // This means that you get a +1 range extension for each 2h in vertical height
        // if you are higher, and a - 1 range penalty for every 2h in vertical height if
        // you are lower.


        return tileList.ToArray();
    }

    public static void Bow_HighlightSelectableRange(CombatUnit caster)
    {
        Engine.TileManager.ClearHighlights();
        Tile[] tiles = Strike_SelectableRange(caster);
        foreach (Tile t in tiles)
        {
            t.Highlight("red_highlight");
        }
    }

    public static int Bow_CTR()
    {
        return 0;
    }

    public static string Bow_PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        return "-" + Bow_PredictedDamage(caster, target).ToString() + "HP " + Bow_PredictedSuccess(caster, target).ToString() + "%";
    }

    private static int Bow_PredictedDamage(CombatUnit caster, CombatUnit target)
    {
        int xa = (caster.PA + caster.Speed) / 2;
        MethodInfo formula = Type.GetType("Attack").GetMethod("Bow_Formula");
        int damage = Statics.Mod2(xa, false, Element.None, caster, target, formula);
        return damage;
    }

    public static int Bow_Formula(int xa, CombatUnit caster)
    {
        return xa * caster.WP();
    }

    private static int Bow_PredictedSuccess(CombatUnit caster, CombatUnit target)
    {
        int baseHit = 100;
        Side side = CombatUnit.actionAngle(caster, target);
        int hitTarget = target.EvadeRate(side, baseHit);
        int value = 100 - hitTarget;

        // See what arc it would take to hit the target
        Vector3 arrowOrigin = caster.transform.position + new Vector3(0, .5f, 0);
        Vector3 arrowTarget = target.transform.position + new Vector3(0, .5f, 0);
        float distance = Vector3.Distance(arrowOrigin, arrowTarget);
        int resolution = (int)Mathf.Ceil(distance * 3);
        int arc = Statics.MinimumArc(arrowOrigin, arrowTarget, 3, resolution);
        if (arc == -1)
        {
            return 0;
        }
        else
        {
            return value;
        }
    }

    public static void Bow_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Bow Timeline").AddComponent<Timeline>();
        t.AdvanceTo(.1f);
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|LongbowAttack", caster.GetComponentInChildren<Animator>());

        // Find out what and when the arrow will hit
        Vector3 arrowOrigin = caster.transform.position + new Vector3(0, .5f, 0);
        Vector3 arrowTarget = targetTile.transform.position + new Vector3(0, .5f, 0);
        float distance = Vector3.Distance(arrowOrigin, arrowTarget);
        int resolution = (int)Mathf.Ceil(distance * 3);
        float arc = Statics.MinimumArc(arrowOrigin, arrowTarget, 3, (int)Mathf.Ceil(distance * 3));

        float arrowFlightDuration = (distance * (arc+1) * .05f);
        Pair<CombatUnit, float> hit = Statics.ProjectileInterrupt(arrowOrigin, arrowTarget, arc, resolution);
        if (arc == 0)
        {
            arc = .25f;
        }
        t.gameObject.AddComponent<Projectile_TimelineEvent>().Init(t, .25f, caster.transform.position + new Vector3(0, 1, 0), targetTile.transform.position + new Vector3(0, 1, 0), arrowFlightDuration, arc, hit.second);
        t.Advance(.25f + arrowFlightDuration * hit.second);

        if (hit.first != null)
        {
            targetUnit = hit.first;
            bool critical = (UnityEngine.Random.Range(1, 100) <= 5);
            int xa = (caster.PA + caster.Speed) / 2;
            MethodInfo formula = Type.GetType("Attack").GetMethod("Bow_Formula");
            int damage = Statics.Mod2(xa, critical, Element.None, caster, targetUnit, formula);
            targetUnit.TakeDamage(damage);

            t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, targetUnit.transform.position, .25f);
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0f, "Armature|Hurt", targetUnit.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0f, "slash");
            t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .25f, damage.ToString(), targetUnit);
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .5f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
            t.Advance(.5f);
        }

        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .25f);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }

    public static void Bow_HighlightAffectedTiles(Tile origin)
    {
        Statics.HighlightTiles(Bow_AffectedTiles(origin));
    }

    public static List<Tile> Bow_AffectedTiles(Tile origin)
    {
        return Statics.AffectedTiles(origin);
    }
    #endregion


}
