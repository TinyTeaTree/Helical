using System;
using System.Threading.Tasks;

namespace Core
{
    public static class TaskExtensions
    {
        public static async void Forget(this Task task)
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
                //Do Nothing Ignore
            }
            catch (Exception exception)
            {
                Notebook.NoteException(exception);
            }
        }
    }
}