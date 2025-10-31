using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public static class TaskUtils
    {
        private static TaskScheduler _unityPlayerLoopScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        public static IEnumerator WaitForCompletion(this Task t)
        {
            while(!t.IsCompleted)
            {
                yield return null;
            }

            if (t.IsFaulted)
            {
                throw t.Exception;
            }
        }

        public static Task ContinueWithMainThread(this Task task, System.Action action)
        {
            task = task.ContinueWith(t =>
            {

                try
                {
                    action?.Invoke();
                }
                catch (Exception e)
                {
                    Notebook.NoteException(e);
                }

            }, _unityPlayerLoopScheduler);

            return task;
        }

        public static async Task WaitUntil(System.Func<bool> untilMethod)
        {
            while (untilMethod() == false)
            {
                await Task.Delay(1);
            }
        }

        public static CancellationTokenSource MakeCT()
        {
            if (Application.isEditor)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
            }
            else
            {
                return new CancellationTokenSource();
            }
        }
    }
}