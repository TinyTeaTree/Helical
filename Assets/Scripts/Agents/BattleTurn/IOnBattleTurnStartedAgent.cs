using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public interface IOnBattleTurnStartedAgent : IAgent
    {
        UniTask OnBattleTurnStarted();
    }
}
