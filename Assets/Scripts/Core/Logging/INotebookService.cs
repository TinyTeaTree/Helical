using System;
using Core;

namespace Core
{
    public interface INotebookService : IService
    {
        void NoteData(string message);
        void NoteWarning(string message);
        void NoteError(string message);
        void NoteException(Exception exception);
        void NoteCritical(string message);
    }
}