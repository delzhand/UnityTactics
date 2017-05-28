using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCurve : MonoBehaviour {
    public float Timer;
    public AnimationCurve Curve;
    public Vector3 OriginalPosition;

    public bool Xpos;
    public bool Ypos;
    public bool Zpos;
    public float Magnitude;

    private void Start()
    {
        OriginalPosition = transform.localPosition;
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        float curveValue = Curve.Evaluate(Timer) * Magnitude;

        Vector3 pos = OriginalPosition;
        if (Xpos)
        {
            pos.x += curveValue;
        }
        if (Ypos)
        {
            pos.y += curveValue;
        }
        if (Zpos)
        {
            pos.z += curveValue;
        }

        transform.localPosition = pos;
    }
}
