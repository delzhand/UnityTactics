using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor
{
    public static void Charge1(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Action action = Engine.CombatManager.ActionTable["Charge1"];

        Timeline t = new GameObject(action.GetName() + " Timeline").AddComponent<Timeline>();

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", action.GetName());
        t.Advance(.15f);

        ActionPatterns.UseWeapon(t, caster, targetTile);

        targetUnit = ActionPatterns.TargetUnit(targetTile);
        if (targetUnit)
        {
            bool critical = Action.GetCritical();
            bool hitSuccess = Action.HitSuccess(caster, targetUnit, 100);
            int damage = Action.Mod2(caster.PA, critical, Element.None, caster, targetUnit, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (hitSuccess)
            {
                ActionPatterns.HitDamage(t, caster, targetUnit, damage);
            }
            else
            {
                ActionPatterns.HitAvoid(t, caster, targetUnit);
            }
        }

        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }
}
