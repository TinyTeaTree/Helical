using Cysharp.Threading.Tasks;
using System;
using UnityEngine;



public abstract class WagAsyncCommand
{
    protected bool _isExecuted;

    public async UniTask Execute()
    {
        if (_isExecuted)
        {
            Debug.LogError($"Command {this.GetType()}, was already executed");
            return;
        }

        _isExecuted = true;
        try
        {
            await InternalExecuteAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in Executing Command {this.GetType().FullName} {e}");
        }
    }

    protected abstract UniTask InternalExecuteAsync();
}
