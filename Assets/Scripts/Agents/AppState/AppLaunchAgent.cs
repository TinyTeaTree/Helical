using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public class AppLaunchAgent : BaseAgent<IAppLaunchAgent>, IAppLaunchAgent
    {
        public UniTask AppLaunch()
        {
            List<UniTask> tasks = new();

            foreach (var receiver in _features)
            {
                tasks.Add(receiver.AppLaunch());
            }

            foreach (var receiver in _services)
            {
                tasks.Add(receiver.AppLaunch());
            }

            return UniTask.WhenAll(tasks);
        }
    }
}