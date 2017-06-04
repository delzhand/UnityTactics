using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolySword : MonoBehaviour
{

    public static string GetName()
    {
        return SettingsManager.PSXTranslation ? "Holy Sword" : "Holy Sword";
    }

    #region Accumulate
    public static string StasisSword_Name()
    {
        return SettingsManager.PSXTranslation ? "Stasis Sword" : "Judgment Blade";
    }

    public static int StasisSword_CTR()
    {
        return 0;
    }

    public static string StasisSword_PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        return "-" + StasisSword_PredictedDamage(caster, target) + " 100%";
    }

    private static int StasisSword_PredictedDamage(CombatUnit caster, CombatUnit target)
    {
        int xa = Statics.Mod2XA(caster.PA, false, Element.None, ZodiacCompatibility.Compare(caster, target));
        int damage = xa * (caster.WP + 2);
        damage = Statics.Mod2Damage(damage);
        return damage;
    }

    public static void StasisSword_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("StasisSword Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", StasisSword_Name());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
        t.Advance(.25f); // Moment of impact

        List<Tile> affected = StasisSword_AffectedTiles(targetTile);
        foreach (Tile affectedTile in affected)
        {
            TileOccupier tO = TileOccupier.GetTileOccupant(affectedTile);
            if (tO != null)
            {
                targetUnit = tO.GetComponent<CombatUnit>();
                bool critical = (Random.Range(1, 100) <= 5);
                int xa = Statics.Mod2XA(caster.PA, critical, Element.None, ZodiacCompatibility.Compare(caster, targetUnit));
                int damage = xa * (caster.WP + 2);
                damage = Statics.Mod2Damage(damage);

                targetUnit.TakeDamage(damage);

                t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, tO.transform.position, .25f);
                t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .25f, "StasisSword", tO.GetOccupiedTile());
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1.25f, "Armature|Hurt", tO.GetComponentInChildren<Animator>());
                t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 1.25f, "clang");
                t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, 2.25f, damage.ToString(), tO);
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 3.75f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 3.75f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
                t.Advance(3.75f);
            }
        }

        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }

    public static void StasisSword_HighlightSelectableRange(CombatUnit caster)
    {
        Engine.TileManager.ClearHighlights();
        Tile[] tiles = StasisSword_SelectableRange(caster);
        foreach(Tile t in tiles)
        {
            t.Highlight("red_highlight");
        }
    }

    public static Tile[] StasisSword_SelectableRange(CombatUnit caster)
    {
        return Statics.RadiusTiles(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile(), 2, false);
    }

    public static void StasisSword_HighlightAffectedTiles(Tile origin)
    {
        Statics.HighlightTiles(StasisSword_AffectedTiles(origin));
    }

    public static List<Tile> StasisSword_AffectedTiles(Tile origin)
    {
        return Statics.AffectedTiles(1, 0, origin, true);
    }
    #endregion

}
