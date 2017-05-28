using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour {

    public static string GetName()
    {
        return SettingsManager.PSXTranslation ? "Charge" : "Aim";
    }

    #region Charge1
    public static string Charge1_Name()
    {
        return SettingsManager.PSXTranslation ? "Charge +1" : "Aim +1";
    }

    public static int Charge1_CTR()
    {
        return 4;
    }

    public static Tile[] Charge1_SelectableRange(CombatUnit caster)
    {
        // @TODO base this on the weapon range, and remove tiles inside the minimum range
        return Engine.TileManager.FindTilesByRadius(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile(), 3, false);
    }

    public static void Charge1_Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Timeline t = new GameObject("Strike Timeline").AddComponent<Timeline>();
        t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, caster.transform.position, .5f);
        t.Advance(.5f);

        t.gameObject.AddComponent<Message_TimelineEvent>().Init(t, 0, "Effect", Charge1_Name());
        t.Advance(.15f);

        t.gameObject.AddComponent<FaceTile_TimelineEvent>().Init(t, 0, caster.GetComponent<Facer>(), targetTile);
        t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0, "Armature|SwordAttack", caster.GetComponentInChildren<Animator>());
        t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "sword_swing");
        t.Advance(.25f); // Moment of impact

        TileOccupier tO = TileOccupier.GetTileOccupant(targetTile);
        if (tO != null)
        {
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 0f, "Armature|Hurt", tO.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<PlaySound_TimelineEvent>().Init(t, 0, "slash");
            t.gameObject.AddComponent<SpawnEffect_TimelineEvent>().Init(t, .2f, "Physical Damage", tO.GetOccupiedTile());
            t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .35f, "23", tO);
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, "Armature|Walk", caster.GetComponentInChildren<Animator>());
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, .55f, "Armature|Walk", tO.GetComponentInChildren<Animator>());
            t.Advance(.55f);
        }
        else
        {
            t.gameObject.AddComponent<UnitAnimation_TimelineEvent>().Init(t, 1f, "Armature|Walk", caster.GetComponentInChildren<Animator>());
        }

        t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, .25f);
        t.PlayFromStart();
    }
    #endregion
}
