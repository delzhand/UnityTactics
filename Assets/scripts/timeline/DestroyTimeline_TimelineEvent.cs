using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimeline_TimelineEvent : TimelineEvent
{
    public Timeline Timeline;

    public void Init(Timeline timeline, float delay)
    {
        SetTime(timeline.CompileTime, delay);
        Timeline = timeline;
    }

    public override void Play()
    {
        base.Play();
        Destroy(Timeline.gameObject);
    }
}
