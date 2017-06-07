using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor {
  public static void NightSword(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Action action = Engine.CombatManager.ActionTable["NightSword"];

        Timeline t = new GameObject("NightSword Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", action.GetName());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
        t.Advance(.25f); // Moment of impact

        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        if (tO != null)
        {
            targetUnit = tO.GetComponent<CombatUnit>();
            MethodInfo formula = Type.GetType("Formulas").GetMethod(action.Id);
            bool critical = (UnityEngine.Random.Range(1, 100) <= 5);
            int damage = Action.Mod2(caster.PA, critical, Element.None, caster, targetUnit, formula);

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
}
