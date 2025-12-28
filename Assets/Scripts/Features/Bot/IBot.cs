using Core;
using Cysharp.Threading.Tasks;
using Agents;

namespace Game
{
    public interface IBot : IFeature, IOnBeforeBattleTurnStartAgent, IOnBattleTurnStartedAgent, IOnBattleTurnEndedAgent
    {
        UniTask OrderBotTurn(BattleUnitData botUnit);
    }
}