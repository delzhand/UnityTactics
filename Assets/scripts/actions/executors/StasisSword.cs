using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor {
    public static void StasisSword(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Action action = Engine.CombatManager.ActionTable["StasisSword"];

        Timeline t = new GameObject("StasisSword Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", action.GetName());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
        t.Advance(.25f); // Moment of impact

        List<Tile> exclude = new List<Tile>();
        exclude.Add(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile());
        List<Tile> affected = action.AffectedTiles(targetTile, exclude);
        foreach (Tile affectedTile in affected)
        {
            TileOccupier tO = TileOccupier.GetTileOccupant(affectedTile);
            if (tO != null)
            {
                targetUnit = tO.GetComponent<CombatUnit>();
                bool critical = (UnityEngine.Random.Range(1, 100) <= 5);
                int damage = Action.Mod2(caster.PA, critical, Element.None, caster, targetUnit, System.Reflection.MethodBase.GetCurrentMethod().Name);

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

}
