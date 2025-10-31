using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WagEvent<T>
{
    string _name;

    List<Action<T>> _actions;
    public List<Action<T>> Actions => _actions ?? (_actions = new List<Action<T>>());

    public static implicit operator System.Action<T>(WagEvent<T> onceEvent) => onceEvent.Invoke;

    public static implicit operator WagEvent<T>(System.Action<T> action)
    {
        var onceEvent = new WagEvent<T>("Implicit Action Event");
        onceEvent += action;
        return onceEvent;
    }

    public WagEvent(string name)
    {
        _name = name;
        _actions = new List<Action<T>>();
    }

    public void AddListener(Action<T> action)
    {
        if (action == null) return;

        Actions.Add(action);
    }

    public UniTask<T> GetAwaitTask()
    {
        UniTaskCompletionSource<T> taskSource = new UniTaskCompletionSource<T>();

        AddListener((t) =>
        {
            taskSource.TrySetResult(t);
        });

        return taskSource.Task;
    }

    public void AddSingle(Action<T> action)
    {
        RemoveListener(action);
        AddListener(action);
    }

    public void AddOnce(Action<T> action)
    {
        if (action == null) return;

        System.Action<T> onceAction = null;
        var thisEvent = this;
        onceAction = (t) =>
        {
            action?.Invoke(t);
            thisEvent.RemoveListener(onceAction);
        };

        AddListener(onceAction);
    }

    public static WagEvent<T> operator +(WagEvent<T> e, System.Action<T> action)
    {
        if (action == null) return e;

        e.Actions.Add(action);
        return e;
    }

    public void RemoveListener(Action<T> action)
    {
        if (_actions == null) return;

        Actions.RemoveAll(a => a == action);
    }

    public static WagEvent<T> operator -(WagEvent<T> e, System.Action<T> action)
    {
        if (e._actions == null) return e;

        e.Actions.RemoveAll(a => a == action);
        return e;
    }

    public void Invoke(T t)
    {
        if (_actions == null) return;

        var clonedActions = new List<Action<T>>(Actions);

        foreach (var action in clonedActions)
        {
            try
            {
                action.Invoke(t);
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
