using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haste : TimedStatus {

    private void Start()
    {
        CTR = 32;
        Slow s = GetComponent<Slow>();
        if (s)
        {
            Destroy(s);
        }
    }

    public int GetCTIncrement(int speed)
    {
        return (int)Math.Truncate(speed * 3 / 2f);
    }

    public override string Name()
    {
        return "Haste";
    }
}
