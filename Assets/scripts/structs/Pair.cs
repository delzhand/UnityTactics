using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Pair<TFirst, TSecond> {
    public TFirst first;
    public TSecond second;

    public Pair(TFirst a, TSecond b) {
        this.first = a;
        this.second = b;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(first != null ? first.ToString() : "Null");
        sb.AppendLine(second != null ? second.ToString() : "Null");
        return sb.ToString();
    }
}
