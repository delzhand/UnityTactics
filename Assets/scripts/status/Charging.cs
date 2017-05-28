using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charging : MonoBehaviour {
    private void Start()
    {
        GetComponentInChildren<Animator>().Play("Armature|Charging");
    }
}
