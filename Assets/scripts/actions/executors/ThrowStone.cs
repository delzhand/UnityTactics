using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor {
    public static void ThrowStone(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Action action = Engine.CombatManager.ActionTable["ThrowStone"];

        Timeline t = new GameObject("Throw Stone Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", action.GetName());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "sword_swing");
        t.Advance(.25f); // Moment of impact


        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        if (tO != null)
        {
            targetUnit = tO.GetComponent<CombatUnit>();

            int damage = Action.Mod2(caster.PA, false, Element.None, caster, targetUnit, System.Reflection.MethodBase.GetCurrentMethod().Name);


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
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        }

        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();


    }


}
