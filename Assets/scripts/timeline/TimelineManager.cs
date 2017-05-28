using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour {
  public bool Clear()
    {
        Timeline t = GetComponentInChildren<Timeline>();
        return (t == null);
    }
}
