using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor
{
    public static void StasisSword(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        string action_id = System.Reflection.MethodBase.GetCurrentMethod().Name;
        Action action = Engine.CombatManager.ActionTable[action_id];

        Timeline t = ActionPatterns.Start(action, true);

        ActionPatterns.UseWeapon(t, caster, targetTile);

        List<Tile> exclude = new List<Tile>();
        exclude.Add(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile());
        List<Tile> affected = action.AffectedTiles(targetTile, exclude);
        foreach (Tile affectedTile in affected)
        {
            targetUnit = ActionPatterns.TargetUnit(targetTile);
            if (targetUnit)
            {
                bool critical = Action.GetCritical();
                int damage = Action.Mod2(caster.PA, critical, Element.None, caster, targetUnit, action_id);

                targetUnit.TakeDamage(damage);

                t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, targetUnit.transform.position, .25f);
                t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .25f, "StasisSword", targetUnit.GetComponent<TileOccupier>().GetOccupiedTile());
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1.25f, "Armature|Hurt", targetUnit.GetComponentInChildren<Animator>());
                t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 1.25f, "clang");
                t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, 2.25f, damage.ToString(), targetUnit);
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 3.75f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
                t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 3.75f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
                t.Advance(3.75f);
            }
        }

        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .25f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }

}
