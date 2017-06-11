using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {

    public float speed = 1;
    public float size = 1;
    private float timer;
    public Vector3 direction = new Vector3(0, 1, 0);
    private Vector3 basePosition = Vector3.zero;
    
	// Use this for initialization
	void Start () {
        basePosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime * speed;
        transform.localPosition = basePosition + (direction * Math.Abs(Mathf.Sin(timer) * size));
    }

    public void UpdateBasePosition(Vector3 update)
    {
        basePosition = update;
    }
}
