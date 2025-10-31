using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Core
{
    public partial class SequenceFlow
    {
        public abstract class Step
        {
            public int Order;
            public float Timeout;
            public string StepId;
            public FlowFailureTolerance FlowFailureTolerance = FlowFailureTolerance.FallBack;

            public async UniTask Execute()
            {
                try
                {
                    if (Timeout == 0)
                    {
                        await InternalExecute();
                    }
                    else
                    {
                        await UniTask.WhenAny(InternalExecute(), UniTask.Delay(TimeSpan.FromSeconds(Timeout)));
                    }
                }
                catch (Exception e)
                {
                    e.Data.Add("failureTolerance", FlowFailureTolerance);

                    switch (FlowFailureTolerance)
                    {
                        case FlowFailureTolerance.ContinueExecution:
                            Notebook.NoteError($"Sequence Flow failed on step, Continue Execution, {StepId ?? GetType().Name}-{Order} {e}");
                            break;
                        case FlowFailureTolerance.FallBack:
                        case FlowFailureTolerance.StopExecution:
                            Notebook.NoteError($"Sequence Flow failed on step {StepId ?? GetType().Name}-{Order} {e}");
                            throw;
                    }
                }
            }

            protected abstract UniTask InternalExecute();
        }
    }
}