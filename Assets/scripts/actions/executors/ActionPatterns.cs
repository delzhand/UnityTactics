using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPatterns : MonoBehaviour {

    public static void UseWeapon(Timeline t, CombatUnit caster, Tile targetTile)
    {
        Weapon w = Engine.CombatManager.WeaponTable[caster.Weapon];
        switch (w.Type)
        {
            case "Sword":
            case "Knife":
                SwingSword(t, caster, targetTile);
                break;
            case "Bow":
                FireBow(t, caster, targetTile);
                break;
        }
    }

    public static void SwingSword(Timeline t, CombatUnit caster, Tile targetTile)
    {
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "sword_swing");
        t.Advance(.25f); // Moment of impact
    }

    public static void FireBow(Timeline t, CombatUnit caster, Tile targetTile)
    {
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|LongbowAttack", caster.GetComponentInChildren<Animator>());

        // Find out what and when the arrow will hit
        Vector3 arrowOrigin = caster.transform.position + new Vector3(0, .5f, 0);
        Vector3 arrowTarget = targetTile.transform.position + new Vector3(0, .5f, 0);
        float distance = Vector3.Distance(arrowOrigin, arrowTarget);
        int resolution = (int)Mathf.Ceil(distance * 3);
        float arc = ParabolicPath.MinimumArc(arrowOrigin, arrowTarget, 3, (int)Mathf.Ceil(distance * 3));

        float arrowFlightDuration = (distance * (arc + 1) * .05f);
        Pair<CombatUnit, float> hit = ParabolicPath.ProjectileInterrupt(arrowOrigin, arrowTarget, arc, resolution);
        if (arc == 0)
        {
            arc = .25f;
        }
        t.gameObject.AddComponent<Projectile_TimelineEvent>().Init(t, .25f, caster.transform.position + new Vector3(0, 1, 0), targetTile.transform.position + new Vector3(0, 1, 0), arrowFlightDuration, arc, hit.second);
        t.Advance(.25f + arrowFlightDuration * hit.second);
    }

    public static CombatUnit TargetUnit(Tile targetTile)
    {
        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        return (tO != null) ? tO.GetComponent<CombatUnit>() : null;
    }

    public static void HitDamage(Timeline t, CombatUnit caster, CombatUnit targetUnit, int damage)
    {
        targetUnit.TakeDamage(damage);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0f, "Armature|Hurt", targetUnit.GetComponentInChildren<Animator>());
        WeaponHitSound(t, caster);
        t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .2f, "PhysicalDamage", targetUnit.GetComponent<TileOccupier>().GetOccupiedTile());
        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, damage.ToString(), targetUnit);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
        t.Advance(.55f);
    }

    public static void WeaponHitSound(Timeline t, CombatUnit caster)
    {
        Weapon w = Engine.CombatManager.WeaponTable[caster.Weapon];
        switch (w.Type)
        {
            default:
                t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
                break;
        }
    }

    public static void HitAvoid(Timeline t, CombatUnit caster, CombatUnit targetUnit)
    {
        EvadeType e = targetUnit.EvadeMethod(CombatUnit.actionAngle(caster, targetUnit));
        switch (e)
        {
            case EvadeType.miss:
                Miss(t, caster, targetUnit);
                break;
            case EvadeType.shield:
                ShieldGuard(t, caster, targetUnit);
                break;
            case EvadeType.weapon:
                WeaponGuard(t, caster, targetUnit);
                break;
            case EvadeType.accessory:
                AccessoryGuard(t, caster, targetUnit);
                break;
        }

    }

    public static void Miss(Timeline t, CombatUnit caster, CombatUnit targetUnit)
    {
        float targetFacing = targetUnit.transform.eulerAngles.y;
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, targetUnit.GetComponent<Facer>(), caster.GetComponent<TileOccupier>().GetOccupiedTile());
        t.gameObject.AddComponent<Shimmy_TimelineEvent>().Init(t, .05f, targetUnit.gameObject);
        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "MISS", targetUnit);
        t.gameObject.AddComponent<FaceAngle_TimelineEvent>().Init(t, .55f, targetUnit.GetComponent<Facer>(), targetFacing);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.Advance(.55f);
    }

    public static void AccessoryGuard(Timeline t, CombatUnit caster, CombatUnit targetUnit)
    {
        float targetFacing = targetUnit.transform.eulerAngles.y;
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, targetUnit.GetComponent<Facer>(), caster.GetComponent<TileOccupier>().GetOccupiedTile());
        t.gameObject.AddComponent<Shimmy_TimelineEvent>().Init(t, .05f, targetUnit.gameObject);
        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "MISS", targetUnit);
        t.gameObject.AddComponent<FaceAngle_TimelineEvent>().Init(t, .55f, targetUnit.GetComponent<Facer>(), targetFacing);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.Advance(.55f);
    }

    public static void ShieldGuard(Timeline t, CombatUnit caster, CombatUnit targetUnit)
    {
        float targetFacing = targetUnit.transform.eulerAngles.y;
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, targetUnit.GetComponent<Facer>(), caster.GetComponent<TileOccupier>().GetOccupiedTile());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|ShieldBlock", targetUnit.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttackBlocked", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "shield_block");
        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "GUARDED", targetUnit);
        t.gameObject.AddComponent<FaceAngle_TimelineEvent>().Init(t, .55f, targetUnit.GetComponent<Facer>(), targetFacing);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .75f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.Advance(.75f);
    }

    public static void WeaponGuard(Timeline t, CombatUnit caster, CombatUnit targetUnit)
    {
        float targetFacing = targetUnit.transform.eulerAngles.y;
        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, targetUnit.GetComponent<Facer>(), caster.GetComponent<TileOccupier>().GetOccupiedTile());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|ShieldBlock", targetUnit.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttackBlocked", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "shield_block");
        t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "GUARDED", targetUnit);
        t.gameObject.AddComponent<FaceAngle_TimelineEvent>().Init(t, .55f, targetUnit.GetComponent<Facer>(), targetFacing);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, targetUnit.GetDefaultAnimation(), targetUnit.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .75f, caster.GetDefaultAnimation(), caster.GetComponentInChildren<Animator>());
        t.Advance(.75f);
    }
}
