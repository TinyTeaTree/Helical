using System;

namespace Core
{
    public static class Notebook
    {
        public static INotebookService NotebookService { get; set; }
        
        public static void NoteData(string message)
        {
            NotebookService.NoteData(message);
        }

        public static void NoteWarning(string message)
        {
            NotebookService.NoteWarning(message);
        }

        public static void NoteError(string message)
        {
            NotebookService.NoteError(message);
        }

        public static void NoteException(Exception exception)
        {
            NotebookService.NoteException(exception);
        }

        public static void NoteCritical(string message)
        {
            NotebookService.NoteCritical(message);
        }
    }
}