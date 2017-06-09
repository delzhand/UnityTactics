using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor
{
    public static void NightSword(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        string action_id = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Action action = Engine.CombatManager.ActionTable[action_id];

        Timeline t = ActionPatterns.Start(action, true);

        ActionPatterns.UseWeapon(t, caster, targetTile);

        targetUnit = ActionPatterns.TargetUnit(targetTile);
        if (targetUnit)
        {
            bool critical = Action.GetCritical();
            int damage = Action.Mod2(caster.PA, critical, Element.None, caster, targetUnit, action_id);

            targetUnit.TakeDamage(damage);

            t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, targetUnit.transform.position, .25f);
            t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .0f, "NightSword", targetUnit.GetComponent<TileOccupier>().GetOccupiedTile());
            t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 2.60f, "clang");
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 2.75f, "Armature|Hurt", targetUnit.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, 4.25f, damage.ToString(), targetUnit);
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 4.25f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 5.25f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 5.25f, caster.transform.position, .25f);
            t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, 5.75f, "+" + damage.ToString(), caster);
            t.Advance(5.75f);
        }

        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .25f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }
}
