using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect_TimelineEvent : TimelineEvent {
    public Vector3 Position;
    public string Name;

    public void Init(Timeline timeline, float delay, string name, Tile tile)
    {
        SetTime(timeline.CompileTime, delay);
        Position = tile.transform.position;
        Name = name;
    }

    public override void Play()
    {
        base.Play();
        GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/effects/" + Name));
        g.transform.position = Position;

    }

}
