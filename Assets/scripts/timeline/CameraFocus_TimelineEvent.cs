using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus_TimelineEvent : TimelineEvent {
    public Vector3 Target;
    public float Duration;

    public void Init(Timeline timeline, float delay, Vector3 target, float duration)
    {
        SetTime(timeline.CompileTime, delay);
        Target = target;
        Duration = duration;
    }

    public override void Play()
    {
        base.Play();
        Engine.CameraManager.SetTargetPosition(Target, Duration);
    }

}
