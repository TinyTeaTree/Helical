using System;
using UnityEngine;


public abstract class BasicCommand<T>
{
    bool _isExecuted;
    public T Execute()
    {
        if (_isExecuted)
        {
            Debug.LogError($"Command {this.GetType()}, was already executed");
            return default;
        }

        _isExecuted = true;
        try
        {
            return InternalExecute();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in Executing Command {this.GetType().FullName} {e.Message}");
        }

        return default;
    }

    protected abstract T InternalExecute();
}
