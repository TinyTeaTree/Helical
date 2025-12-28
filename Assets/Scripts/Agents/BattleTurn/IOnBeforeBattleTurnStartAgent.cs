using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public interface IOnBeforeBattleTurnStartAgent : IAgent
    {
        UniTask OnBeforeBattleTurnStart();
    }
}
