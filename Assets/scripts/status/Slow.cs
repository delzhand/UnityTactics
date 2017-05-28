using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : TimedStatus {

    private void Start()
    {
        CTR = 24;
        Haste h = GetComponent<Haste>();
        if (h)
        {
            Destroy(h);
        }

    }

    public int GetCTIncrement(int speed)
    {
        return (int)Math.Truncate(speed / 2f);
    }

    public override string Name()
    {
        return "Slow";
    }
}
