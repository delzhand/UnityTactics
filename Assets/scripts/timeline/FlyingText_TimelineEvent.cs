using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingText_TimelineEvent : TimelineEvent {
    public string Text;
    public MonoBehaviour Target;

    public void Init(Timeline timeline, float delay, string text, MonoBehaviour target)
    {
        SetTime(timeline.CompileTime, delay);
        Text = text;
        Target = target;
    }

    public override void Play()
    {
        base.Play();
        FlyingText.CreateFromCharacters(Text, Target, 0);
    }
}
