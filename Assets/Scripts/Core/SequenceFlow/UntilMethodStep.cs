using Cysharp.Threading.Tasks;

namespace Core
{
    public partial class SequenceFlow
    {
        public class UntilMethodStep : Step
        {
            private System.Func<bool> _untilMethod;
            
            public UntilMethodStep(System.Func<bool> untilMethod)
            {
                _untilMethod = untilMethod;
            }

            protected override async UniTask InternalExecute()
            {
                while (!_untilMethod())
                {
                    await UniTask.Yield(); //next frame
                }
            }
        }
    }
}