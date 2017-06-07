using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SlowAction : MonoBehaviour {

    public int CTR;
    public CombatUnit Caster;
    public Tile TargetTile;
    public CombatUnit TargetUnit;
    public string Action;
    public bool Executing = false;

    public void DecrementCTR()
    {
        CTR = Math.Max(0, CTR - 1);
    }

    public void Execute()
    {
        Executing = true;
        Type.GetType("Executor").GetMethod(Action).Invoke(null, new object[]{ Caster, TargetTile, TargetUnit });
        Destroy(this);
    }
}
