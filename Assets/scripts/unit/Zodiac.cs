using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Zodiac
{
    Aries,
    Taurus,
    Gemini,
    Cancer,
    Leo,
    Virgo,
    Libra,
    Scorpio,
    Sagittarius,
    Capricorn,
    Aquarius,
    Pisces,
    Serpentarius
}

public enum Compatibility
{
    Neutral,
    Good,
    Best,
    Bad,
    Worst
}

public class ZodiacCompatibility
{
    public static int Modify(int xa, CombatUnit a, CombatUnit b)
    {
        Compatibility compat = ZodiacCompatibility.Compare(a, b);
        if (compat == Compatibility.Best)
        {
            xa += xa / 2;
        }
        else if (compat == Compatibility.Good)
        {
            xa += xa / 4;
        }
        else if (compat == Compatibility.Bad)
        {
            xa -= xa / 4;
        }
        else if (compat == Compatibility.Worst)
        {
            xa -= xa / 2;
        }
        return xa;
    }

    public static Compatibility Compare(CombatUnit a, CombatUnit b)
    {
        return Compare(a.Zodiac, b.Zodiac, a.Gender, b.Gender);
    }

    public static Compatibility Compare(Zodiac az, Zodiac bz, Gender ag, Gender bg)
    {
        int c = (int)az - (int)bz;

        if (c < 0)
        {
            c += 12;
        }

        if (az == Zodiac.Serpentarius || bz == Zodiac.Serpentarius)
        {
            return Compatibility.Neutral;
        }
        else if (c == 3 || c == 9)
        {
            return Compatibility.Bad;
        }
        else if (c == 4 || c == 8)
        {
            return Compatibility.Good;
        }
        else if (c == 6)
        {
            if (ag == Gender.Monster || bg == Gender.Monster)
            {
                return Compatibility.Bad;
            }
            if (ag == bg)
            {
                return Compatibility.Worst;
            }
            else
            {
                return Compatibility.Best;
            }
        }
        else
        {
            return Compatibility.Neutral;
        }
    }

    //public static void TestZC()
    //{
    //    foreach (Zodiac a in Enum.GetValues(typeof(Zodiac)))
    //    {
    //        StringBuilder s = new StringBuilder();
    //        s.Append(a.ToString() + ": ");
    //        foreach (Zodiac b in Enum.GetValues(typeof(Zodiac)))
    //        {
    //            int c = (int)a - (int)b;

    //            if (c < 0)
    //            {
    //                c += 12;
    //            }

    //            if (a == Zodiac.Serpentarius || b == Zodiac.Serpentarius)
    //            {
    //                s.Append("0");
    //            }
    //            else if (c == 3 || c == 9)
    //            {
    //                s.Append("-");
    //            }
    //            else if (c == 4 || c == 8)
    //            {
    //                s.Append("+");
    //            }
    //            else if (c == 6)
    //            {
    //                s.Append("?");
    //            }
    //            else
    //            {
    //                s.Append("0");
    //            }
    //        }
    //        Debug.Log(s.ToString());
    //    }
    //}
}
