using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_TimelineEvent : TimelineEvent {

    public Vector3 Origin;
    public Vector3 Target;
    public float Arc;
    public float FlightDuration;
    public float StopAt;

    public void Init(Timeline timeline, float delay, Vector3 origin, Vector3 target, float flightDuration, float arc, float stopAt)
    {
        SetTime(timeline.CompileTime, delay);
        Origin = origin;
        Target = target;
        Arc = arc;
        StopAt = stopAt;
        FlightDuration = flightDuration;
    }

    public override void Play()
    {
        base.Play();
        ParabolicPath p = new GameObject("Parabola").AddComponent<ParabolicPath>();
        p.FlightDuration = FlightDuration;
        p.Origin = Origin;
        p.Target = Target;
        p.Arc = Arc;
        p.StopAt = StopAt;

        GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/effects/Arrow"));
        p.Projectile = g;
        g.transform.parent = p.transform;

    }

}
