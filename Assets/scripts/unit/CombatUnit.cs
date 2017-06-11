using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum Gender
{
    Male,
    Female,
    Monster
}

public enum EvadeType
{
    miss,
    shield,
    weapon,
    accessory
}

public class CombatUnit : MonoBehaviour {

    public int Order;
    public int Move;
    public int Jump;
    public int Speed;
    public int CT;
    public Zodiac Zodiac;
    public Gender Gender;
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
    public int MA;
    public int ClassEvade;
    public int ShieldEvade;
    public int AccessoryEvade;
    public string Weapon;

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
        m.Top = .3f;
        m.Left = .1f;

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

    public int WeaponEvade()
    {
        if (false) // equipped with Weapon Guard
        {
            // I don't think there's ever a case where you can dual wield weapons with evade...
            return Engine.CombatManager.WeaponTable[Weapon].WEv;
        }
        return 0;
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
                evade += WeaponEvade();
                break;
            case Side.Side:
                evade += ShieldEvade;
                evade += AccessoryEvade;
                evade += WeaponEvade();
                break;
            case Side.Back:
                evade += AccessoryEvade;
                break;
        }
        return evade;
    }

    public EvadeType EvadeMethod(Side side)
    {
        int totalEvade = 0;
        int pick = 0;
        switch(side)
        {
            case Side.Front:
                totalEvade = ClassEvade + ShieldEvade + AccessoryEvade + WeaponEvade();
                pick = UnityEngine.Random.Range(1, totalEvade);
                if (pick <= ClassEvade)
                {
                    return EvadeType.miss;
                }
                if (pick <= ClassEvade + AccessoryEvade)
                {
                    return EvadeType.accessory;
                }
                else if (pick <= ClassEvade + AccessoryEvade + ShieldEvade)
                {
                    return EvadeType.shield;
                }
                else if (pick <= ClassEvade + AccessoryEvade + ShieldEvade + WeaponEvade())
                {
                    return EvadeType.weapon;
                }
                break;
            case Side.Side:
                totalEvade = ShieldEvade + AccessoryEvade + WeaponEvade();
                pick = UnityEngine.Random.Range(1, totalEvade);
                if (pick <= AccessoryEvade)
                {
                    return EvadeType.accessory;
                }
                else if (pick <= AccessoryEvade + ShieldEvade)
                {
                    return EvadeType.shield;
                }
                else if (pick <= AccessoryEvade + ShieldEvade + WeaponEvade())
                {
                    return EvadeType.weapon;
                }
                break;
            case Side.Back:
                // only accessory evade applies from behind, this one is easy
                return EvadeType.accessory;
        }
        throw new Exception("Side not handled.");
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

    public int WP()
    {
        if (Weapon.Length > 0)
        {
            return Engine.CombatManager.WeaponTable[Weapon].WP;
        }
        return 0;
    }
}
