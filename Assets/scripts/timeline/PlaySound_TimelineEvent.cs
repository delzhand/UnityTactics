using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_TimelineEvent : TimelineEvent {
    public string Sound;

    public void Init(Timeline timeline, float delay, string sound)
    {
        SetTime(timeline.CompileTime, delay);
        Sound = sound;
    }

    public override void Play()
    {
        base.Play();
        Engine.SoundManager.PlaySound(Sound);
    }
}
