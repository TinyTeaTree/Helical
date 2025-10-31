using System.Collections.Generic;

namespace Core
{
    public partial class SequenceFlow
    {
        private class StepGroup
        {
            private List<Step> _steps = new List<Step>();

            public IEnumerable<Step> AllSteps => _steps;

            public void AddStep(Step step)
            {
                _steps.Add(step);
            }
        }
    }
}