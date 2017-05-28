using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedStatus : MonoBehaviour {

    public int CTR = 0;

    public void DecrementClocktick()
    {
        CTR = Math.Max(0, CTR - 1);
    }

    public virtual string Name()
    {
        return "";
    }

}
