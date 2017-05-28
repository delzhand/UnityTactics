using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightSword : MonoBehaviour {
    public float Timer;
    public AnimationCurve Sword;
    public Vector3 OriginalPosition;

    private void Start()
    {
        OriginalPosition = transform.Find("Sword").transform.position;
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        float curveValue = Sword.Evaluate(Timer);
        Vector3 position = new Vector3(0, 2f * curveValue, 0);
        transform.Find("Sword").transform.position = OriginalPosition + position;
        transform.Find("Sword").transform.localScale = 15 * new Vector3(curveValue, curveValue, curveValue);
        if (Timer > Sword[Sword.length -1].time)
        {
            Destroy(gameObject);
        }
    }
}
