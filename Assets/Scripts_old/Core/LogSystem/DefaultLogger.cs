using System;


public class DefaultLogger : ILog
{
    public virtual void Critical(string critical)
    {
        UnityEngine.Debug.Log(critical);
    }

    public virtual void Editor(string editorTrace)
    {
#if UNITY_EDITOR || !PRODUCTION
        UnityEngine.Debug.Log(editorTrace);
#endif
    }

    public virtual void Error(string error)
    {
        UnityEngine.Debug.LogError(error);
    }

    public virtual void Exception(Exception e)
    {
        UnityEngine.Debug.LogException(e);
    }

    public virtual void Message(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    public virtual void Warning(string warning)
    {
        UnityEngine.Debug.LogWarning(warning);
    }
}