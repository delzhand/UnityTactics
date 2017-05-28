using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTile_TimelineEvent : TimelineEvent {
    public Facer Facer;
    public Tile Tile;

    public void Init(Timeline timeline, float delay, Facer facer, Tile tile)
    {
        SetTime(timeline.CompileTime, delay);
        Facer = facer;
        Tile = tile;
    }

    public override void Play()
    {
        base.Play();
        Facer.FaceTile(Tile);
    }
}
