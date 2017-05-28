using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour {

    public Animator Animator;

    // Defaults may ONLY be:
    // walk, stand, injured, dead, charging, casting
    public string DefaultAnimation = "Armature|Walk";
    List<Pair<float, string>> PendingAnimations;

    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        PendingAnimations = new List<Pair<float, string>>();
        Animator.Play(DefaultAnimation, -1, Random.Range(0.0f, 1.08f));
    }

    private void Update()
    {
        List<Pair<float, string>> toRemove = new List<Pair<float, string>>();

        foreach(Pair<float, string> p in PendingAnimations)
        {
            p.first -= Time.deltaTime;
            if (p.first <= 0)
            {
                toRemove.Add(p);
                Animator.Play(p.second);
            }
        }
        foreach(Pair<float, string> p in toRemove)
        {
            PendingAnimations.Remove(p);
        }
    }

    public void AddPendingAnimation(float delay, string animation)
    {
        PendingAnimations.Add(new Pair<float, string>(delay, animation));
    }

    public void RestoreDefault(float delay)
    {
        PendingAnimations.Add(new Pair<float, string>(delay, DefaultAnimation));
    }

}
