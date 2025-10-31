using System;
using Core;

namespace Services
{
    public class NotebookService : BaseService, INotebookService
    {
        public void NoteData(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }

        public void NoteWarning(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(message);
#endif
        }

        public void NoteError(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(message);
#endif
        }

        public void NoteException(Exception exception)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(exception);
#endif
        }

        public void NoteCritical(string message)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(message);
#endif
        }
    }
}