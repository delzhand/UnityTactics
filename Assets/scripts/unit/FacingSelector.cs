using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingSelector : MonoBehaviour {

    private float timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime * 10;
        float value = Mathf.Sin(timer);

        Color c = new Color(Mathf.Lerp(.5f, 1, value), Mathf.Lerp(.4f, .8f, value), 0, 1);
        GetComponent<MeshRenderer>().materials[0].color = c;
	}


}
