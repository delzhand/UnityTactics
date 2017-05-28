using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shimmy_TimelineEvent : TimelineEvent {
    public GameObject GameObject;

    public void Init(Timeline timeline, float delay, GameObject gameObject)
    {
        SetTime(timeline.CompileTime, delay);
        GameObject = gameObject;
    }

    public override void Play()
    {
        base.Play();
        Shimmy s = GameObject.AddComponent<Shimmy>();
        s.direction = 1;
    }
}
