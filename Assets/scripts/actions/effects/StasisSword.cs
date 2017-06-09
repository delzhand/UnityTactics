using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StasisSword : MonoBehaviour {

    public float Timer;

    private void Update()
    {
        Timer += Time.deltaTime;

        if (Timer > 1f)
        {
            transform.Find("big crystal").GetComponent<MeshRenderer>().enabled = true;
            transform.Find("small crystal 1").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("small crystal 2").GetComponent<MeshRenderer>().enabled = false;
            transform.Find("small crystal 3").GetComponent<MeshRenderer>().enabled = false;
        }

        if (Timer > 3f)
        {
            Destroy(gameObject);
        }
    }
}
