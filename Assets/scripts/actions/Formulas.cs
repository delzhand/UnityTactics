using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formulas {
    public static int Attack(int xa, CombatUnit caster)
    {
        if (caster.Weapon.Length == 0)
        {
            // bare hands
            return xa * caster.PA;
        }
        else
        {
            return xa * caster.WP();
        }
    }
    public static int Charge1(int xa, CombatUnit caster)
    {
        return Charge(xa, caster, 1);
    }
    public static int Charge(int xa, CombatUnit caster, int charge)
    {
        return Attack(xa, caster);
    }
    public static int NightSword(int xa, CombatUnit caster)
    {
        return xa * caster.WP();
    }
    public static int StasisSword(int xa, CombatUnit caster)
    {
        return xa * (caster.WP() + 2);
    }
    public static int ThrowStone(int xa, CombatUnit caster)
    {
        return xa * UnityEngine.Random.Range(1, 2);
    }
}
