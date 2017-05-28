using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum Zodiac
{
    Aquarius,
    Pisces,
    Aries,
    Taurus,
    Gemini,
    Cancer,
    Leo,
    Virgo,
    Libra,
    Scorpio,
    Sagittarius,
    Capricorn
}

public class CombatUnit : MonoBehaviour {

    public int Order;
    public int Move;
    public int Jump;
    public int Speed;
    public int CT;
    public Zodiac Zodiac;
    public string Class;
    public int Brave;
    public int Faith;
    public int Level;
    public int Exp;
    public int HP;
    public int MP;
    public int MaxHP;
    public int MaxMP;
    public int PA;
    public int WP;
    public int ClassEvade;
    public int ShieldEvade;
    public int AccessoryEvade;
    public int WeaponEvade;

    public bool Guest;
    public bool Enemy;

    public int GetCTIncrement()
    {
        Haste h = gameObject.GetComponent<Haste>();
        if (h != null)
        {
            return h.GetCTIncrement(Speed);
        }
        return Speed;
    }

    public string GetDefaultAnimation()
    {
        if (GetComponent<KO>())
        {
            return "Armature|KO";
        }
        if (GetComponent<Critical>())
        {
            return "Armature|Critical";
        }
        return "Armature|Walk";
    }

    public string GetVitals()
    {
        StringBuilder s = new StringBuilder();
        s.AppendLine("HP " + Bar(HP, MaxHP, 20) + " " + HP + "/" + MaxHP);
        s.AppendLine("MP " + Bar(MP, MaxMP, 20) + " " + MP + "/" + MaxMP);
        s.AppendLine("CT " + Bar(CT, 100, 20) + " " + Math.Min(100, CT) + "/100");
        return s.ToString();
    }

    public string GetCombatInfo()
    {
        return Order + "/" + Engine.CombatManager.Units.Length + "  Lv." + Level + "  Exp." + Exp;
    }

    private string Bar(int value, int max, int bar_length)
    {
        string bar = "";
        float percent = value / (float)max;
        for(int i = 0; i < bar_length; i++)
        {
            if (i <= bar_length * percent)
            {
                bar += "|";
            }
            else
            {
                bar += ".";
            }
        }
        return bar;
    }

    public void ShowMenu()
    {
        Menu m = new GameObject().AddComponent<Menu>();
        m.gameObject.tag = "Menus";
        m.name = gameObject.name + " Command";
        m.Attach = gameObject;

        ActiveTurn a = GetComponent<ActiveTurn>();
        if (a && !Guest && !Enemy)
        {
            m.AddMenuOption("Move", m.gameObject.AddComponent<Move_MenuOption>());
            m.AddMenuOption("Act", m.gameObject.AddComponent<Act_MenuOption>());
            m.AddMenuOption("Wait", m.gameObject.AddComponent<Wait_MenuOption>());
        }
        m.AddMenuOption("Status");
        if (!Guest && !Enemy)
        {
            m.AddMenuOption("Auto-battle");
        }
        m.ReturnControlTo = Engine.MapCursor;
    }

    public void TakeDamage(int damage)
    {
        HP = Mathf.Max(0, HP - damage);
        if (HP == 0)
        {
            KO k = GetComponent<KO>();
            if (k == null)
            {
                gameObject.AddComponent<KO>();
            }
        }
        else if (HP < Math.Ceiling(MaxHP/5f)) {
            Critical c = GetComponent<Critical>();
            if (c == null)
            {
                gameObject.AddComponent<Critical>();
            }
        }
    }

    public int EvadeRate(Side side, int baseHit)
    {
        int evade = 100 - baseHit;
        switch(side)
        {
            case Side.Front:
                evade += ClassEvade;
                evade += ShieldEvade;
                evade += AccessoryEvade;
                evade += WeaponEvade;
                break;
            case Side.Side:
                evade += ShieldEvade;
                evade += AccessoryEvade;
                evade += WeaponEvade;
                break;
            case Side.Back:
                evade += AccessoryEvade;
                break;
        }
        return evade;
    }

    // 0 = miss, 1 = shield, 2 = weapon
    public int EvadeType(Side side)
    {
        int totalEvade = 0;
        int pick = 0;
        switch(side)
        {
            case Side.Front:
                totalEvade = ClassEvade + ShieldEvade + AccessoryEvade + WeaponEvade;
                pick = UnityEngine.Random.Range(1, totalEvade);
                if (pick <= ClassEvade + AccessoryEvade)
                {
                    return 0;
                }
                else if (pick <= ClassEvade + AccessoryEvade + ShieldEvade)
                {
                    return 1;
                }
                else if (pick <= ClassEvade + AccessoryEvade + ShieldEvade + WeaponEvade)
                {
                    return 2;
                }
                break;
            case Side.Side:
                totalEvade = ShieldEvade + AccessoryEvade + WeaponEvade;
                pick = UnityEngine.Random.Range(1, totalEvade);
                if (pick <= AccessoryEvade)
                {
                    return 0;
                }
                else if (pick <= AccessoryEvade + ShieldEvade)
                {
                    return 1;
                }
                else if (pick <= AccessoryEvade + ShieldEvade + WeaponEvade)
                {
                    return 2;
                }
                break;
            case Side.Back:
                // only accessory evade applies from behind, this one is easy
                return 0;
        }
        throw new Exception("Side not handled.");
    }

    public static int Mod2XA(int xa, bool critical, Element element)
    {
        // Critical
        if (critical)
        {
            xa += UnityEngine.Random.Range(1, (xa - 1));
        }

        // Caster Elemental Strengthen

        // Caster Attack Up support

        // Caster Martial Arts

        // Caster Berserk

        // Target Defense Up

        // Target Protect

        // Target Charging

        // Target Sleeping

        // Target Chicken/Frog

        // Caster+Target Zodiac

        return xa;
    }

    public static int Mod2Damage(int damage)
    {
        // Target Elemental Weak

        // Target Elemental Halved

        // Target Elemental Absorb

        return damage;
    }

    public static Side actionAngle(CombatUnit caster, CombatUnit target)
    {
        // We're going to cheat a little bit here. Instead of doing calculations based
        // on tile occupation, we're just going to use world space transform checking.
        // The outcome should be identical.
        Vector3 direction = target.transform.position - caster.transform.position;
        float RawAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        RawAngle -= target.transform.eulerAngles.y;
        RawAngle = Math.Abs(RawAngle);

        if (RawAngle < 45)
        {
            return Side.Back;
        }
        else if (RawAngle < 135)
        {
            return Side.Side;
        }
        else
        {
            return Side.Front;
        }
    }
}
