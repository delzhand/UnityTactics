using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


[Serializable]
public class Action {
    public string Id;
    public string CommandSet;
    public string PSXName;
    public string PSPName;
    public string SelectableRange;
    public int RangeH = 1;
    public int RangeV = -1;
    public string EffectPattern;
    public int EffectH = 1;
    public int EffectV = 1;
    public string Element;
    public int CTR;
    public int Mod;
    public int Mp;
    public bool Evadeable;
    public bool CasterImmune;

    public string GetName()
    {
        return SettingsManager.PSXTranslation ? PSXName : PSPName;
    }

    public void HighlightSelectableTiles(CombatUnit caster)
    {
        Tile origin = caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile();

        List<Tile> tiles = new List<Tile>();
        if (SelectableRange == "Auto")
        {
            tiles.Add(origin);
        }
        else if (SelectableRange == "Weapon")
        {
            throw new NotImplementedException("Weapon range selection not implemented");
            // Striking
            // Piercing
            // RangedArc
            // RangedStraight
        }
        else
        {
            // Default pattern is a simple radius
            Statics.RadiusTiles(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile(), 2, false);
            Tile[] radius = Engine.TileManager.FindTilesByRadius(origin, RangeH, !CasterImmune);
            foreach (Tile t in radius)
            {
                if (RangeV == -1)
                {
                    // When vertical range is -1, there's no limit
                    tiles.Add(t);
                }
                else
                {
                    float heightDiff = Math.Abs(t.GetEffectiveHeight() - origin.GetEffectiveHeight());
                    if (heightDiff <= RangeV)
                    {
                        tiles.Add(t);
                    }

                }
            }
        }

        Material m = Resources.Load("graphics/materials/utility/red_highlight", typeof(Material)) as Material;
        foreach (Tile tile in tiles)
        {
            tile.Highlight(m);
            tile.CurrentlySelectable = true;
        }

    }

    internal string PredictedEffect(CombatUnit caster, CombatUnit target)
    {
        throw new NotImplementedException();
    }

    internal void HighlightAffectedTiles(Tile origin, CombatUnit caster)
    {
        List<Tile> excludes = new List<Tile>();
        if (CasterImmune)
        {
            excludes.Add(caster.gameObject.GetComponent<TileOccupier>().GetOccupiedTile());
        }
        List<Tile> tiles = AffectedTiles(origin, excludes);

        Material m = Resources.Load("graphics/materials/utility/yellow_highlight", typeof(Material)) as Material;
        foreach (Tile tile in tiles)
        {
            tile.Highlight(m);
        }
    }

    public List<Tile> AffectedTiles(Tile origin, List<Tile> excludes)
    {
        List<Tile> tiles = new List<Tile>();
        if (EffectPattern == "Linear")
        {
            throw new NotImplementedException();
        }
        else
        {
            // Default pattern is simple radius
            tiles = AffectedRadius(origin, excludes);
        }
        return tiles;
    }

    public List<Tile> AffectedRadius(Tile origin, List<Tile> excludes)
    {
        List<Tile> tiles = new List<Tile>();
        // Radius is horizontal effect minus one because we're including the origin
        Tile[] range = Engine.TileManager.FindTilesByRadius(origin, EffectH - 1, true);
        foreach (Tile t in range)
        {
            if (!excludes.Contains(t))
            {
                if (Mathf.Abs(origin.Height - t.Height) <= EffectV)
                {
                    tiles.Add(t);
                }
            }
        }
        return tiles;
    }

    public bool CanBeCast(CombatUnit caster)
    {
        return true;
    }

    public void Execute(CombatUnit caster, Tile targetTile, CombatUnit targetUnit)
    {
        Type.GetType("Executor").GetMethod(Id).Invoke(null, new object[] { caster, targetTile, targetUnit });
    }

    public static int Mod2(int xa, bool critical, Element element, CombatUnit caster, CombatUnit target, MethodInfo formula)
    {
        // Critical
        if (critical)
        {
            xa += UnityEngine.Random.Range(1, (xa - 1));
        }

        // Caster Elemental Strengthen

        // Caster Attack Up support

        // Caster Martial Arts

        // Caster Berserk

        // Target Defense Up

        // Target Protect

        // Target Charging

        // Target Sleeping

        // Target Chicken/Frog

        // Caster+Target Zodiac
        xa = ZodiacCompatibility.Modify(xa, caster, target);

        int damage = (int)formula.Invoke(null, new object[] { xa, caster });

        // Target Elemental Weak

        // Target Elemental Halved

        // Target Elemental Absorb

        return damage;
    }

    public static int Mod5(Element element, int spellPower, CombatUnit caster, CombatUnit target)
    {
        int damage = 0;
        float frac = 1;
        int xa = caster.MA;

        // Caster Elemental Strengthen

        // Caster Magic Up

        // Target Magic Defend Up

        // Target Shell

        // Caster+Target Zodiac
        xa = ZodiacCompatibility.Modify(xa, caster, target);

        // Caster/Target Faith status

        // Weather Elemental effect

        // Target Elemental Weakness

        // Target Elemental Half

        // Target Elemental Absorb

        damage = (int)(caster.Faith * target.Faith * spellPower * xa * frac / 10000f);

        return damage;
    }

}
