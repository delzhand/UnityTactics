using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmp : MonoBehaviour {
    public GameObject Caster;
    public float RawAngle;
    public Side Side;

    private void Update()
    {
        Vector3 direction = transform.position - Caster.transform.position;
        RawAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        RawAngle -= transform.eulerAngles.y;
        RawAngle = Math.Abs(RawAngle);

        if (RawAngle < 45)
        {
            Side = Side.Back;
        }
        else if (RawAngle < 135)
        {
            Side = Side.Side;
        }
        else
        {
            Side = Side.Front;
        }

        //public static int actionAngle(CombatUnit caster, CombatUnit target)
        //{
        //    // We're going to cheat a little bit here. Instead of doing calculations based
        //    // on tile occupation, we're just going to use world space transform checking.
        //    // The outcome should be identical.

        //    Vector3 direction = target.transform.eulerAngles - caster.transform.eulerAngles;
        //    float angle = Mathf.Atan2()

        //    Mathf.Atan2(target.transform.eulerAngles.y, )
        //    TileOccupier cTO = caster.GetComponent<TileOccupier>();
        //    TileOccupier tTO = target.GetComponent<TileOccupier>();

        //    int forwardOffset = target

        //}
    }
}
