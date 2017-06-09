using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class Executor
{
    public static void AttackSword(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Attack (Sword) Timeline").AddComponent<Timeline>();
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
            MethodInfo formula = Type.GetType("Formulas").GetMethod("Attack");
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
    public static void AttackBow(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Attack (Bow) Timeline").AddComponent<Timeline>();
        t.AdvanceTo(.1f);
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|LongbowAttack", caster.GetComponentInChildren<Animator>());

        // Find out what and when the arrow will hit
        Vector3 arrowOrigin = caster.transform.position + new Vector3(0, .5f, 0);
        Vector3 arrowTarget = targetTile.transform.position + new Vector3(0, .5f, 0);
        float distance = Vector3.Distance(arrowOrigin, arrowTarget);
        int resolution = (int)Mathf.Ceil(distance * 3);
        float arc = Statics.MinimumArc(arrowOrigin, arrowTarget, 3, (int)Mathf.Ceil(distance * 3));

        float arrowFlightDuration = (distance * (arc + 1) * .05f);
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
            MethodInfo formula = Type.GetType("Formulas").GetMethod("Attack");
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
}
