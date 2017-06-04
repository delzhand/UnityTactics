using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackMagic : MonoBehaviour {

    public static string GetName()
    {
        return SettingsManager.PSXTranslation ? "Black Magic" : "Black Magicks";
    }

    #region Fire
    public static string Fire_Name()
    {
        return "Fire";
    }

    public static Tile[] Fire_SelectableRange(CombatUnit caster)
    {
        return Engine.TileManager.FindTilesByRadius(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile(), 4, false);
    }

    public static void Fire_Highlight_SelectableRange(CombatUnit caster)
    {
        Engine.TileManager.ClearHighlights();
        Tile[] tiles = Fire_SelectableRange(caster);
        foreach (Tile t in tiles)
        {
            t.Highlight("red_highlight");
        }
    }

    public static string Fire_PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        int damage = Statics.Mod5(Element.Fire, 14, caster, target);
        return "-" + damage.ToString() + " 100%";
    }

    public static void Fire_HighlightAffectedTiles(Tile origin)
    {
        Statics.HighlightTiles(Fire_AffectedTiles(origin));
    }

    public static List<Tile> Fire_AffectedTiles(Tile origin)
    {
        return Statics.AffectedTiles(1, 0, origin, true);
    }

    public static int Fire_CTR()
    {
        return 4;
    }

    public static void Fire_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Fire Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", Fire_Name());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.Advance(.25f); // Moment of impact

        List<Tile> affected = Fire_AffectedTiles(targetTile);
        foreach (Tile affectedTile in affected)
        {
            TileOccupier tO = TileOccupier.GetTileOccupant(affectedTile);
            if (tO != null)
            {
                targetUnit = tO.GetComponent<CombatUnit>();
                bool critical = (Random.Range(1, 100) <= 5);
                int damage = Statics.Mod5(Element.Fire, 14, caster, targetUnit);

                targetUnit.TakeDamage(damage);

                t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, tO.transform.position, .25f);
                t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .25f, "Fire", tO.GetOccupiedTile());
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1.25f, "Armature|Hurt", tO.GetComponentInChildren<Animator>());
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
    #endregion

}
