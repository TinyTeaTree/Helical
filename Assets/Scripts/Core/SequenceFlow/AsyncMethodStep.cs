using System;
using Cysharp.Threading.Tasks;

namespace Core
{
    public partial class SequenceFlow
    {
        public class AsyncMethodStep : Step
        {
            private Func<UniTask> _asyncMethod;
            
            public AsyncMethodStep(Func<UniTask> asyncMethod)
            {
                _asyncMethod = asyncMethod;
            }

            protected override async UniTask InternalExecute()
            {
                await _asyncMethod();
            }
        }
    }
}