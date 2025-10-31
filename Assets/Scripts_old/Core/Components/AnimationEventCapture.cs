using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCapture : MonoBehaviour
{
    private Dictionary<string, System.Action> _callbackDictionary = new();

    public void SetCallback(string arg, System.Action callback)
    {
        _callbackDictionary[arg] = callback;
    }

    public void RemoveCallback(string arg)
    {
        _callbackDictionary.Remove(arg);
    }

    public void TriggetAnimationEvent(string arg)
    {
        if(_callbackDictionary.TryGetValue(arg, out var callback))
        {
            callback.Invoke();
        }
        else
        {
            Debug.LogWarning($"Did not find any callback to invoke for {arg}");
        }
    }
}