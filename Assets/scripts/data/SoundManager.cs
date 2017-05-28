using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private Dictionary<string, AudioClip> audioClips;
    public List<Pair<float, string>> PendingSFX;

    private void Awake()
    {
        audioClips = new Dictionary<string, AudioClip>();
        PendingSFX = new List<Pair<float, string>>();
    }

    private void Update()
    {
        List<Pair<float, string>> toRemove = new List<Pair<float, string>>();

        foreach (Pair<float, string> p in PendingSFX)
        {
            p.first -= Time.deltaTime;
            if (p.first <= 0)
            {
                toRemove.Add(p);
                PlaySound(p.second);
            }
        }
        foreach (Pair<float, string> p in toRemove)
        {
            PendingSFX.Remove(p);
        }

    }

    public void AddPendingSoundEffect(float delay, string name)
    {
        PendingSFX.Add(new Pair<float, string>(delay, name));
    }

    public void PlaySound(string name)
    {
        audioClips[name] = (AudioClip)Resources.Load<AudioClip>("sound/fx/" + name);

        Engine.CameraManager.Camera.GetComponent<AudioSource>().PlayOneShot(audioClips[name]);
    }
}
