using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public class OnBattleTurnStartedAgent : BaseAgent<IOnBattleTurnStartedAgent>, IOnBattleTurnStartedAgent
    {
        public UniTask OnBattleTurnStarted()
        {
            List<UniTask> tasks = new();

            foreach (var receiver in _features)
            {
                tasks.Add(receiver.OnBattleTurnStarted());
            }

            foreach (var receiver in _services)
            {
                tasks.Add(receiver.OnBattleTurnStarted());
            }

            return UniTask.WhenAll(tasks);
        }
    }
}
