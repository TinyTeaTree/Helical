using Core;
using Cysharp.Threading.Tasks;
using Agents;

namespace Game
{
    public interface IBot : IFeature, IAppLaunchAgent, IOnBeforeBattleTurnStartAgent, IOnBattleTurnStartedAgent, IOnBattleTurnEndedAgent
    {
    }
}