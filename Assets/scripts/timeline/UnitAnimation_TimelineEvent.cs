using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation_TimelineEvent : TimelineEvent {
    public string Animation;
    public Animator Animator;

    public void Init(Timeline timeline, float delay, string animation, Animator animator)
    {
        SetTime(timeline.CompileTime, delay);
        Animation = animation;
        Animator = animator;
    }

    public override void Play()
    {
        base.Play();
        Animator.Play(Animation);
    }
}
