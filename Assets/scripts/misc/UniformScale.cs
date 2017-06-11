using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniformScale : MonoBehaviour {
    public float Scale = 1;

    public void SetScale(float f)
    {
        Scale = f;
        transform.localScale = new Vector3(f, f, f);
    }
}
