using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimelineEvent : MonoBehaviour {
    public float StartReference = 0;
    public float StartOffset = 0;
    public bool Started = false;

    public virtual void Play()
    {
        Started = true;
    }

    public void SetTime(float reference, float offset)
    {
        StartReference = reference;
        StartOffset = offset;
    }
}
