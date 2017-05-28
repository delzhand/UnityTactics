using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFlash : MonoBehaviour {

    private float timer;

    public float DelayMax = 10;
    public float DelayMin = 5;
    public float MaxIntensity = 6;
    public float IntensityFalloff = .9f;
    private float nextFlash = 0;
    private Light lightningLight;

	// Use this for initialization
	void Start () {
        setNextFlash();
        lightningLight = gameObject.GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > nextFlash)
        {
            lightningLight.intensity = MaxIntensity;
            setNextFlash();
        }

        if (lightningLight.intensity > .001f)
        {
            lightningLight.intensity *= IntensityFalloff;
        }
    }

    private void setNextFlash()
    {
        nextFlash = timer + Random.Range(DelayMin, DelayMax);
    }
}
