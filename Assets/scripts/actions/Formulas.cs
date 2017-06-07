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
}
