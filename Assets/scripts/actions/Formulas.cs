using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formulas {
    public static int NightSword(int xa, CombatUnit caster)
    {
        return xa * caster.WP;
    }
    public static int StasisSword(int xa, CombatUnit caster)
    {
        return xa * (caster.WP + 2);
    }
    public static int ThrowStone(int xa, CombatUnit caster)
    {
        return xa * UnityEngine.Random.Range(1, 2);
    }
}
