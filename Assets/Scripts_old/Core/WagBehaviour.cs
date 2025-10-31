using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class WagBehaviour : MonoBehaviour
{
    bool _dead;
    public bool IsDead => this == null || _dead;

    protected virtual bool ShouldAutoPopulate { get; } = false;

    struct EventRegistration
    {
        public WagEvent TaleEvent;
        public Action Callback;

        public void Unregister()
        {
            TaleEvent -= Callback;
        }
    }

    List<EventRegistration> _registeredEvents = new();
    Dictionary<int, Coroutine> _routines = new Dictionary<int, Coroutine>();

    protected virtual void Awake()
    {
        if (ShouldAutoPopulate)
        {
            AutoPopulate();
        }
    }

    protected void StartWagRoutine(System.Func<IEnumerator> routine)
    {
        var routineHash = routine.GetHashCode();
        if(_routines.TryGetValue(routineHash, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        _routines[routineHash] = StartCoroutine(routine());
    }

    void AutoPopulate()
    {
        var myType = GetType();
        var fieldsOutoPopulate = myType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<AutoPopulateAttribute>() != null);

        foreach (var toPopulateField in fieldsOutoPopulate)
        {
            var fieldType = toPopulateField.FieldType;

            var component = GetComponent(fieldType);

            toPopulateField.SetValue(this, component);
        }
    }

    protected void StopWagRoutine(System.Func<IEnumerator> routine)
    {
        var routineHash = routine.GetHashCode();
        if (_routines.TryGetValue(routineHash, out Coroutine coroutine))
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        _routines[routineHash] = null;
    }

    protected void RegisterEvent(WagEvent taleEvent, System.Action callback)
    {
        taleEvent += callback;
        
        _registeredEvents.Add(new EventRegistration()
        {
            Callback = callback,
            TaleEvent = taleEvent
        });
    }
    
    protected void InvokeAfter(float delay, System.Action action)
    {
        StartCoroutine(InvokeAfterRoutine(delay, action));
    }

    IEnumerator InvokeAfterRoutine(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    protected virtual void OnDestroy()
    {
        foreach (var registration in _registeredEvents)
        {
            registration.Unregister();
        }
        
        _registeredEvents.Clear();
        _dead = true;
    }
}
