using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Core
{
    public partial class SequenceFlow
    {
        public class TaskStep : Step
        {
            private UniTask _task;

            public TaskStep(UniTask task)
            {
                _task = task;
            }

            protected override async UniTask InternalExecute()
            {
                await _task;
            }
        }
    }
}