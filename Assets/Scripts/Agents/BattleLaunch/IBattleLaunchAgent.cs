using Core;
using Cysharp.Threading.Tasks;

namespace Agents
{
    public interface IBattleLaunchAgent : IAgent
    {
        UniTask BattleLaunch();
    }
}

