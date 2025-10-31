using System;
using System.Collections.Generic;
using UnityEngine;


public struct WagOnceEvent
{
    string _name;

    List<Action> _actions;
    public List<Action> Actions => _actions ?? (_actions = new List<Action>());

    public static implicit operator System.Action(WagOnceEvent onceEvent) => onceEvent.Invoke;

    public static implicit operator WagOnceEvent(System.Action action)
    {
        var onceEvent = new WagOnceEvent("Implicit Action Event");
        onceEvent += action;
        return onceEvent;
    }

    public WagOnceEvent(string name)
    {
        _name = name;
        _actions = new List<Action>();
    }

    public void AddListener(Action action)
    {
        if (action == null) return;

        Actions.Add(action);
    }

    public static WagOnceEvent operator +(WagOnceEvent e, System.Action action)
    {
        if (action == null) return e;

        e.Actions.Add(action);
        return e;
    }

    public void RemoveListener(Action action)
    {
        if (_actions == null) return;

        Actions.RemoveAll(a => a == action);
    }

    public static WagOnceEvent operator -(WagOnceEvent e, System.Action action)
    {
        if (e._actions == null) return e;

        e.Actions.RemoveAll(a => a == action);
        return e;
    }

    public void Invoke()
    {
        if (_actions == null) return;

        var clonedActions = new List<Action>(Actions);

        foreach (var action in clonedActions)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to invoke {_name} callback {action.Method} in event {e}");
            }
        }

        RemoveAllListeners();
    }

    public void RemoveAllListeners()
    {
        _actions?.Clear();
    }
}
