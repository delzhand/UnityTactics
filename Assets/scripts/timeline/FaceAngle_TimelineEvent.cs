using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAngle_TimelineEvent : TimelineEvent {

    public float Angle;
    public Facer Facer;

    public void Init(Timeline t, float delay, Facer facer, float angle)
    {
        SetTime(t.CompileTime, delay);
        Facer = facer;
        Angle = angle;
    }

    public override void Play()
    {
        base.Play();
        Facer.transform.eulerAngles = new Vector3(Facer.transform.eulerAngles.x, Angle, Facer.transform.eulerAngles.z);
    }
}
