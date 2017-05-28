using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinIdle : MonoBehaviour {

    private float timer = 0;
    private float speed = 5.4f;

    private static float armswing = .2f;
    private static float legswing = .4f;

	// Use this for initialization
	void Start () {
        // Desync animations
        timer += Random.Range(0, Mathf.PI * 2);
	}
	
	// Update is called once per frame
	void Update () {
        Haste h = gameObject.GetComponent<Haste>();
        if (h != null)
        {
            timer += Time.deltaTime * speed * 1.8f;
        }
        else
        {
            timer += Time.deltaTime * speed;
        }

        float sin = (Mathf.Sin(timer) + 1) / 2f;
        float doubleCos = (Mathf.Cos(timer));

        transform.Find("Body").localPosition = Vector3.Lerp(new Vector3(0, .835f, 0), new Vector3(0, .874f, 0), Mathf.Abs(doubleCos));

        transform.Find("Body/R Leg").localPosition = Vector3.Lerp(new Vector3(-0.236f, -1.138f, -legswing), new Vector3(-0.236f, -1.138f, legswing), sin);
        transform.Find("Body/R Leg").localEulerAngles = Vector3.Lerp(new Vector3(16f, 0, 0), new Vector3(-16f, 0, 0), sin);
        transform.Find("Body/L Leg").localPosition = Vector3.Lerp(new Vector3(0.236f, -1.138f, legswing), new Vector3(0.236f, -1.138f, -legswing), sin);
        transform.Find("Body/L Leg").localEulerAngles = Vector3.Lerp(new Vector3(-16f, 0, 0), new Vector3(16f, 0, 0), sin);

        transform.Find("Body/R Arm").localPosition = Vector3.Lerp(new Vector3(0.694f, -0.089f, -armswing), new Vector3(0.694f, -0.089f, armswing), sin);
        transform.Find("Body/R Arm").localEulerAngles = Vector3.Lerp(new Vector3(16f, 0, 0), new Vector3(-16f, 0, 0), sin);
        transform.Find("Body/L Arm").localPosition = Vector3.Lerp(new Vector3(-0.694f, -0.089f, armswing), new Vector3(-0.694f, -0.089f, -armswing), sin);
        transform.Find("Body/L Arm").localEulerAngles = Vector3.Lerp(new Vector3(-16f, 0, 0), new Vector3(16f, 0, 0), sin);

    }
}
