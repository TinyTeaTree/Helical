using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public class OnBattleTurnEndedAgent : BaseAgent<IOnBattleTurnEndedAgent>, IOnBattleTurnEndedAgent
    {
        public UniTask OnBattleTurnEnded()
        {
            List<UniTask> tasks = new();

            foreach (var receiver in _features)
            {
                tasks.Add(receiver.OnBattleTurnEnded());
            }

            foreach (var receiver in _services)
            {
                tasks.Add(receiver.OnBattleTurnEnded());
            }

            return UniTask.WhenAll(tasks);
        }
    }
}
