using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {

    public float speed = 1;
    public float size = 1;
    private float timer;
    private Vector3 basePosition = Vector3.zero;

	// Use this for initialization
	void Start () {
        basePosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime * speed;
        transform.localPosition = basePosition + new Vector3(0, Math.Abs(Mathf.Sin(timer) * size), 0);

        transform.rotation = Camera.main.transform.rotation;
    }
}
