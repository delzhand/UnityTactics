using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSkill : MonoBehaviour {

    public static string GetName()
    {
        return SettingsManager.PSXTranslation ? "Basic Skill" : "Fundaments";
    }

    #region Throw Stone 
    public static string ThrowStone_Name()
    {
        return SettingsManager.PSXTranslation ? "Throw Stone" : "Stone";
    }

    public static int ThrowStone_CTR()
    {
        return 0;
    }
    public static string ThrowStone_PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        return "-" + ThrowStone_PredictedDamage(caster, target).ToString() + "HP " + ThrowStone_PredictedSuccess(caster, target).ToString() + "%";
    }

    private static int ThrowStone_PredictedDamage(CombatUnit caster, CombatUnit target)
    {
        int xa = Statics.Mod2XA(caster.PA, false, Element.None, caster, target);
        int damage = xa * Random.Range(1, 2);
        damage = Statics.Mod2Damage(damage);
        return damage;
    }

    private static int ThrowStone_PredictedSuccess(CombatUnit caster, CombatUnit target)
    {
        int baseHit = 100;
        Side side = CombatUnit.actionAngle(caster, target);
        int hitTarget = target.EvadeRate(side, baseHit);
        return 100 - hitTarget;
    }

    public static void ThrowStone_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Throw Stone Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", ThrowStone_Name());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "sword_swing");
        t.Advance(.25f); // Moment of impact


        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        if (tO != null)
        {
            targetUnit = tO.GetComponent<CombatUnit>();

            int xa = Statics.Mod2XA(caster.PA, false, Element.None, caster, targetUnit);
            int damage = xa * Random.Range(1,2);
            damage = Statics.Mod2Damage(damage);


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

    public static Tile[] ThrowStone_SelectableRange(CombatUnit caster)
    {
        return Engine.TileManager.FindTilesByRadius(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile(), 4, false);
    }

    #endregion

}
