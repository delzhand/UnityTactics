using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Action {
    public string Id;
    public string CommandSet;
    public string PSXName;
    public string PSPName;
    public string RangeH;
    public string RangeV;
    public string EffectH = "1";
    public string EffectV = "1";
    public string Element;
    public int CTR = 0;
    public int Mod;
    public int Mp;
    public bool Evadeable;
    public bool CasterImmune;

    public string GetName()
    {
        return SettingsManager.PSXTranslation ? PSXName : PSPName;
    }

    public int GetRangeH(CombatUnit caster)
    {
        if (RangeH == "")
        {
            return 1;
        }
        if (RangeH == "Weapon")
        {
            return caster.WeaponRange();
        }

        return int.Parse(RangeH);
    }
}
