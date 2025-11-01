using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public class BattleLaunchAgent : BaseAgent<IBattleLaunchAgent>, IBattleLaunchAgent
    {
        public UniTask BattleLaunch()
        {
            List<UniTask> tasks = new();

            foreach (var receiver in _features)
            {
                tasks.Add(receiver.BattleLaunch());
            }

            return UniTask.WhenAll(tasks);
        }
    }
}

