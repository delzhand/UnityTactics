using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase
{
    StatusCheck,
    StatusCheckResolution,
    SlowActionCharging,
    SlowActionResolution,
    CTCharging,
    ActiveTurnResolution
}

public class CombatManager : MonoBehaviour {

    public Phase Phase = Phase.StatusCheck;
    public CombatUnit[] Units;
    public int ProcessingUnit = 0;
    public bool Paused;

    public Dictionary<string, Action> ActionTable;
    public Dictionary<string, CommandSet> CommandSetTable;
    public Dictionary<string, Weapon> WeaponTable;

    public bool DevMode = false;

    void Start() {
        Units = orderUnits();
        Action[] actions = JsonHelper.FromJson<Action>(Resources.Load<TextAsset>("text/lists/actions").text);
        ActionTable = new Dictionary<string, Action>();
        foreach (Action a in actions)
        {
            ActionTable.Add(a.Id, a);
        }

        CommandSet[] commandSets = JsonHelper.FromJson<CommandSet>(Resources.Load<TextAsset>("text/lists/commandSets").text);
        CommandSetTable = new Dictionary<string, CommandSet>();
        foreach (CommandSet c in commandSets)
        {
            CommandSetTable.Add(c.Id, c);
        }

        Weapon[] weapons = JsonHelper.FromJson<Weapon>(Resources.Load<TextAsset>("text/lists/weapons").text);
        foreach (Weapon w in weapons)
        {
            WeaponTable.Add(w.Id, w);
        }
    }

    void Update () {
        int loopProtection = 0;
        while (!Paused)
        {
            loopProtection++;
            if (loopProtection > 1000)
            {
                Paused = true;
                Debug.LogError("Too many loops without an engine event.");
            }
            switch (Phase)
            {
                case Phase.StatusCheck:
                    statusCheck();
                    break;
                case Phase.StatusCheckResolution:
                    statusCheckResolution();
                    break;
                case Phase.SlowActionCharging:
                    slowActionCharging();
                    break;
                case Phase.SlowActionResolution:
                    slowActionResolution();
                    break;
                case Phase.CTCharging:
                    ctCharging();
                    break;
                case Phase.ActiveTurnResolution:
                    activeTurnResolution();
                    break;
            }
            // If in dev mode we force stepping through combat one update at a time by pausing at the end of every update
            if (DevMode)
            {
                Paused = true;
            }
        }

        //processEvents();
        if (Phase == Phase.StatusCheckResolution || Phase == Phase.SlowActionResolution)
        {
            if (Engine.TimelineManager.Clear())
            {
                Engine.CombatManager.Unpause();
            }
        }
    }

    public void Unpause()
    {
        Paused = false;
    }

    private void statusCheck()
    {
        foreach(CombatUnit c in Units)
        {
            if (c.GetComponent<Permadeath>() == null)
            {
                TimedStatus[] statuses = c.GetComponents<TimedStatus>();
                foreach(TimedStatus ts in statuses)
                {
                    ts.DecrementClocktick();
                }
            }
        }
        Phase = Phase.StatusCheckResolution;
    }

    private void statusCheckResolution()
    {
        foreach(CombatUnit c in Units)
        {
            TimedStatus[] statuses = c.GetComponents<TimedStatus>();
            foreach (TimedStatus ts in statuses)
            {
                if (ts.CTR <= 0)
                {
                    // Generate a simple timeline to focus the unit and show the status removal
                    Timeline t = new GameObject("Status Cleared").AddComponent<Timeline>();
                    t.gameObject.AddComponent<CameraFocus_TimelineEvent>().Init(t, 0, c.transform.position, .5f);
                    t.gameObject.AddComponent<FlyingText_TimelineEvent>().Init(t, .5f, "-" + ts.Name(), c);
                    t.gameObject.AddComponent<DestroyTimeline_TimelineEvent>().Init(t, 1f);
                    t.PlayFromStart();
                    Destroy(ts);
                    Paused = true;
                    return;
                }
            }

        }
        Phase = Phase.SlowActionCharging;
    }

    private void slowActionCharging()
    {
        SlowAction[] slowActions = GetComponents<SlowAction>();
        foreach (SlowAction sa in slowActions)
        {
            sa.DecrementCTR();
        }
        Phase = Phase.SlowActionResolution;
    }

    private void slowActionResolution()
    {
        SlowAction[] slowActions = GetComponents<SlowAction>();
        foreach(SlowAction sa in slowActions)
        {
            if (sa.CTR <= 0 && !sa.Executing)
            {
                sa.Execute();
                Paused = true;
                return;
            }
        }
        Phase = Phase.CTCharging;
    }

    private void ctCharging()
    {
        foreach(CombatUnit c in Units)
        {
            c.CT += c.GetCTIncrement();
        }
        Phase = Phase.ActiveTurnResolution;
    }

    private void activeTurnResolution()
    {
        foreach(CombatUnit c in Units)
        {
            if (c.CT >= 100)
            {
                c.gameObject.AddComponent<ActiveTurn>().Activate();
                Paused = true;
                return;
            }
        }
        Phase = Phase.StatusCheck;
    }

    private CombatUnit[] orderUnits()
    {
        SortedDictionary<int, CombatUnit> sorted = new SortedDictionary<int, CombatUnit>();
        foreach (CombatUnit cu in GetComponentsInChildren<CombatUnit>())
        {
            sorted.Add(cu.Order, cu);
        }
        CombatUnit[] cus = new CombatUnit[sorted.Count];
        sorted.Values.CopyTo(cus, 0);
        return cus;
    }
}
