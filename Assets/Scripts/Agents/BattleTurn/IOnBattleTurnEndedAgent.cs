using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public interface IOnBattleTurnEndedAgent : IAgent
    {
        UniTask OnBattleTurnEnded();
    }
}
