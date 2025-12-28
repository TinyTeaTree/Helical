using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public class OnBeforeBattleTurnStartAgent : BaseAgent<IOnBeforeBattleTurnStartAgent>, IOnBeforeBattleTurnStartAgent
    {
        public UniTask OnBeforeBattleTurnStart()
        {
            List<UniTask> tasks = new();

            foreach (var receiver in _features)
            {
                tasks.Add(receiver.OnBeforeBattleTurnStart());
            }

            foreach (var receiver in _services)
            {
                tasks.Add(receiver.OnBeforeBattleTurnStart());
            }

            return UniTask.WhenAll(tasks);
        }
    }
}
