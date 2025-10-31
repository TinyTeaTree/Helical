using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Core
{
    public partial class SequenceFlow
    {
        public class ActionStep : Step
        {
            private System.Action _action;

            public ActionStep(System.Action action)
            {
                _action = action;
            }

            protected override UniTask InternalExecute()
            {
                _action();
                return UniTask.CompletedTask;
            }
        }
    }
}