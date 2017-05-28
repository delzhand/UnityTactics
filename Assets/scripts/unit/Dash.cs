using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    public float timer;
    public float distance = .5f;
    public float duration = .25f;
    public float delay = 0;
    public int direction = 0;
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Split the duration in half
        if (timer >= delay)
        {
            float progressTime = timer - delay;
            float progressPercent = progressTime / duration;
            float progress = Mathf.Sin(Mathf.PI * progressPercent);
            Vector3 offset = Vector3.zero;
            float lerp = Mathf.Lerp(0, distance, progress);
            if (direction == 0)
            {
                offset.z = lerp;
            }
            else if (direction == 1)
            {
                offset.x = lerp;
            }
            else if (direction == 2)
            {
                offset.z = -lerp;
            }
            else if (direction == 3)
            {
                offset.x = -lerp;
            }
            transform.position = originalPosition + offset;
        }
        if (timer >= delay + duration)
        {
            Destroy(this);
        }
    }
}
