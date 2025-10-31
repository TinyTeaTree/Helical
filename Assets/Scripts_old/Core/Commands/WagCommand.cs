
public abstract class WagCommand<T> : BasicCommand
    where T : WagCommand<T>
{
    bool _isDone;
    WagEvent _onDoneEvent = new WagEvent($"On Done {nameof(T)}");

    public T WithOnDone(System.Action onDoneAction)
    {
        if(_isExecuted)
        {
            UnityEngine.Debug.LogError($"Command {GetType()}, was already executed");
            return this as T;
        }

        _onDoneEvent.AddListener(onDoneAction);

        return this as T;
    }

    protected void SetDone()
    {
        if (_isDone)
        {
            UnityEngine.Debug.LogError($"Command {GetType()}, was already done");
            return;
        }

        _isDone = true;
        _onDoneEvent.Invoke();
        _onDoneEvent.RemoveAllListeners();
    }
}