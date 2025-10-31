using System;
using UnityEngine;


public abstract class BasicCommand
{
    protected bool _isExecuted;

    public void Execute()
    {
        if (_isExecuted)
        {
            Debug.LogError($"Command {this.GetType()}, was already executed");
            return;
        }

        _isExecuted = true;
        try
        {
            InternalExecute();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in Executing Command {this.GetType().FullName} {e}");
        }
    }

    protected abstract void InternalExecute();
}
