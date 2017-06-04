using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkSword : MonoBehaviour {

    public static string GetName()
    {
        return SettingsManager.PSXTranslation ? "Dark Sword" : "Fell Sword";
    }

    #region Accumulate
    public static string NightSword_Name()
    {
        return SettingsManager.PSXTranslation ? "Night Sword" : "Shadowblade";
    }

    public static int NightSword_CTR()
    {
        return 0;
    }

    public static string NightSword_PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        return "-" + NightSword_PredictedDamage(caster, target) + " 100%";
    }

    private static int NightSword_PredictedDamage(CombatUnit caster, CombatUnit target)
    {
        int xa = Statics.Mod2XA(caster.PA, false, Element.None, ZodiacCompatibility.Compare(caster, target));
        int damage = xa * caster.WP;
        damage = Statics.Mod2Damage(damage);
        return damage;
    }

    public static void NightSword_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("NightSword Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", NightSword_Name());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
        t.Advance(.25f); // Moment of impact

        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        if (tO != null)
        {
            targetUnit = tO.GetComponent<CombatUnit>();
            bool critical = (Random.Range(1, 100) <= 5);
            int xa = Statics.Mod2XA(caster.PA, critical, Element.None, ZodiacCompatibility.Compare(caster, targetUnit));
            int damage = xa * caster.WP;
            damage = Statics.Mod2Damage(damage);

            targetUnit.TakeDamage(damage);

            t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, tO.transform.position, .25f);
            t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .0f, "NightSword", tO.GetOccupiedTile());
            t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 2.60f, "clang");
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 2.75f, "Armature|Hurt", tO.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, 4.25f, damage.ToString(), tO);
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 4.25f, targetUnit.GetDefaultAnimation(), tO.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 5.25f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 5.25f, caster.transform.position, .25f);
            t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, 5.75f, "+" + damage.ToString(), caster);
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 5.75f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
            t.Advance(6.75f);
        }
        else
        {
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        }

        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }

    public static Tile[] NightSword_SelectableRange(CombatUnit Caster)
    {
        return Engine.TileManager.FindTilesByRadius(Caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile(), 3, false);
    }

    public static void NightSword_HighlightAffectedTiles(Tile origin)
    {
        Statics.HighlightTiles(NightSword_AffectedTiles(origin));
    }

    public static List<Tile> NightSword_AffectedTiles(Tile origin)
    {
        return Statics.AffectedTiles(origin);
    }
    #endregion

}
