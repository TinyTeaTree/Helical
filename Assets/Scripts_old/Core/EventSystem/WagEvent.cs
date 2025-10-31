using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WagEvent
{
    string _name;

    List<Action> _actions;
    public List<Action> Actions => _actions ?? (_actions = new List<Action>());

    public static implicit operator System.Action(WagEvent onceEvent) => onceEvent.Invoke;

    public static implicit operator WagEvent(System.Action action)
    {
        var onceEvent = new WagEvent("Implicit Action Event");
        onceEvent += action; 
        return onceEvent;
    }

    public WagEvent(string name)
    {
        _name = name;
        _actions = new List<Action>();
    }

    public void AddListener(Action action)
    {
        if (action == null) return;

        Actions.Add(action);
    }

    public UniTask GetAwaitTask()
    {
        UniTaskCompletionSource taskSource = new UniTaskCompletionSource();

        AddListener(() =>
        {
            taskSource.TrySetResult();
        });

        return taskSource.Task;
    }

    public void AddSingle(Action action)
    {
        RemoveListener(action);
        AddListener(action);
    }

    public void AddOnce(Action action)
    {
        if (action == null) return;

        System.Action onceAction = null;
        var thisEvent = this;
        onceAction = () =>
        {
            action?.Invoke();
            thisEvent.RemoveListener(onceAction);
        };

        AddListener(onceAction);
    }

    public static WagEvent operator +(WagEvent e, System.Action action)
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

    public static WagEvent operator -(WagEvent e, System.Action action)
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
    }

    public void RemoveAllListeners()
    {
        _actions?.Clear();
    }
}
