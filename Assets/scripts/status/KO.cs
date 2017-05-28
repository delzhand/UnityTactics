using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KO : MonoBehaviour {
    public int Counter = 4;
    private void Start()
    {
        Critical c = GetComponent<Critical>();
        if (c)
        {
            Destroy(c);
        }

    }

}
