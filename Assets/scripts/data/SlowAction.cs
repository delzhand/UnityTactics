using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SlowAction : MonoBehaviour {

    public int CTR;
    public Type Type;
    public CombatUnit Caster;
    public Tile TargetTile;
    public CombatUnit TargetUnit;
    public string Action;
    public bool Executing = false;

    public void DecrementCTR()
    {
        CTR = Math.Max(0, CTR - 1);
    }

    //public float ExecutionTime()
    //{
    //    return (float)Type.GetMethod(Action + "_ExecutionTime").Invoke(null, new object[] { Caster, TargetTile, TargetUnit });
    //}

    public void Execute()
    {
        Executing = true;
        Type.GetMethod(Action + "_Execute").Invoke(null, new object[]{ Caster, TargetTile, TargetUnit});
        Destroy(this);
        //Invoke("PostExecute", ExecutionTime() + 1f);
    }

    //public void PostExecute()
    //{
    //    Destroy(this);
    //}
}
