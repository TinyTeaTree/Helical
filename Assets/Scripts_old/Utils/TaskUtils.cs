using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ChessRaid
{
    public static class TaskUtils
    {
        private static RoutineRunner _runner;
        private static RoutineRunner Runner
        {
            get
            {
                if(_runner == null)
                {
                    var go = new GameObject("Routine Runner");
                    _runner = go.AddComponent<RoutineRunner>();
                }

                return _runner;
            }
        }

        public static async void Forget(this Task task)
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
                //Skip
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static async Task WaitYieldInstruction(YieldInstruction yieldInstruction)
        {
            bool done = false;
            System.Action doneAction = () => { done = true; };
            var coroutine = Runner.StartCoroutine(RunYield(yieldInstruction, doneAction));
            while (!done)
            {
                await Task.Delay(1); //wait 1 frame
            }
        }

        private static IEnumerator RunYield(YieldInstruction yieldInstruction, System.Action doneAction)
        {
            yield return yieldInstruction;
            doneAction.Invoke();
        }

        public static async Task WaitUntil(System.Func<bool> condition)
        {
            while (!condition())
                await Task.Delay(1);
        }
    }
}