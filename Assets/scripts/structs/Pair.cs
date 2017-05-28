using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<TFirst, TSecond> {
    public TFirst first;
    public TSecond second;

    public Pair(TFirst a, TSecond b) {
        this.first = a;
        this.second = b;
    }
}
