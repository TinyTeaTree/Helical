using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Core
{
    public partial class SequenceFlow
    {
        private List<StepGroup> _stepGroups = new List<StepGroup>();
        private int _order = 1;

        private SequenceFlow _fallbackFlow;

        public static SequenceFlow Create()
        {
            return new SequenceFlow();
        }

        protected SequenceFlow(){}

        public SequenceFlow AddFallback(SequenceFlow fallback)
        {
            _fallbackFlow = fallback;
            return this;
        }
        
        public SequenceFlow AddNext(UniTask task, string stepId = null, float timeout = 0)
        {
            StepGroup group = new StepGroup();
            group.AddStep(new TaskStep(task) { Order = _order++, Timeout = timeout, StepId = stepId });
            _stepGroups.Add(group);
            return this;
        }

        public SequenceFlow AddNext(Func<UniTask> asyncMethod, string stepId = null, float timeout = 0)
        {
            StepGroup group = new StepGroup();
            group.AddStep(new AsyncMethodStep(asyncMethod) { Order = _order++, Timeout = timeout, StepId = stepId });
            _stepGroups.Add(group);
            return this;
        }

        public SequenceFlow AddNext(System.Action action, string stepId = null, float timeout = 0)
        {
            StepGroup group = new StepGroup();
            group.AddStep(new ActionStep(action) { Order = _order++, Timeout = timeout, StepId = stepId });
            _stepGroups.Add(group);
            return this;
        }

        public SequenceFlow AddNext(System.Func<bool> untilMethod, string stepId = null, float timeout = 0)
        {
            StepGroup group = new StepGroup();
            group.AddStep(new UntilMethodStep(untilMethod) { Order = _order++, Timeout = timeout, StepId = stepId });
            _stepGroups.Add(group);
            return this;
        }
        
        public SequenceFlow AddParallel(UniTask task, string stepId = null, float timeout = 0)
        {
            if (_stepGroups.Count == 0)
            {
                AddNext(task);
            }
            else
            {
                _stepGroups.Last().AddStep(new TaskStep(task) { Order = _order++, Timeout = timeout, StepId = stepId });
            }
            return this;
        }
        
        public SequenceFlow AddParallel(Func<UniTask> asyncMethod, string stepId = null, float timeout = 0)
        {
            if (_stepGroups.Count == 0)
            {
                AddNext(asyncMethod);
            }
            else
            {
                _stepGroups.Last().AddStep(new AsyncMethodStep(asyncMethod) { Order = _order++, Timeout = timeout, StepId = stepId });
            }
            return this;
        }

        public SequenceFlow AddParallel(System.Action action, string stepId = null, float timeout = 0)
        {
            if (_stepGroups.Count == 0)
            {
                AddNext(action);
            }
            else
            {
                _stepGroups.Last().AddStep(new ActionStep(action) { Order = _order++, Timeout = timeout, StepId = stepId });
            }
            return this;
        }

        public SequenceFlow AddParallel(System.Func<bool> untilMethod, string stepId = null, float timeout = 0)
        {
            if (_stepGroups.Count == 0)
            {
                AddNext(untilMethod);
            }
            else
            {
                _stepGroups.Last().AddStep(new UntilMethodStep(untilMethod) { Order = _order++, Timeout = timeout, StepId = stepId});
            }
            return this;
        }

        public SequenceFlow WithTolerance(FlowFailureTolerance tolerance)
        {
            var lastGroup = _stepGroups.Last();

            lastGroup.AllSteps.Last().FlowFailureTolerance = tolerance;

            return this;
        }

        public void Execute()
        {
            ExecuteAsync().Forget();
        }

        public async UniTask ExecuteAsync()
        {
            try
            {
                foreach (var group in _stepGroups)
                {
                    var allParallelSteps = group.AllSteps;
                    await UniTask.WhenAll(Enumerable.Select(allParallelSteps, step => step.Execute()));
                }
            }
            catch (Exception e)
            {
                var tolerance = (FlowFailureTolerance)e.Data["failureTolerance"];

                switch (tolerance)
                {
                    case FlowFailureTolerance.FallBack:
                        if (_fallbackFlow != null)
                        {
                            Notebook.NoteData($"Executing fallback flow due to exception {e}");
                            await _fallbackFlow.ExecuteAsync();
                        }
                        break;
                    case FlowFailureTolerance.StopExecution:
                        Notebook.NoteError($"Halting execution due to flow exception {e}");
                        return;
                }
            }
        }
    }
}