using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : MonoBehaviour {

    public float CompileTime = 0;
    public float CurrentTime = 0;
    public bool Play = false;

    private void Awake()
    {
        transform.parent = Engine.TimelineManager.transform;
    }

    private void Update()
    {
        if (Play)
        {
            TimelineEvent[] tes = GetComponents<TimelineEvent>();

            CurrentTime += Time.deltaTime;
            foreach (TimelineEvent te in tes)
            {
                if (CurrentTime >= te.StartReference + te.StartOffset && te.Started == false)
                {
                    te.Play();
                }
            }
        }
    }

    public void AdvanceTo(float time)
    {
        CompileTime = time;
    }

    public void Advance(float time)
    {
        CompileTime += time;
    }

    public void PlayFromStart()
    {
        CurrentTime = 0;
        Play = true;
        TimelineEvent[] tes = GetComponents<TimelineEvent>();
        foreach (TimelineEvent te in tes)
        {
            te.Started = false;
        }
    }
}
