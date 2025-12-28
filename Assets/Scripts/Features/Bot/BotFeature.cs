using Core;
using Cysharp.Threading.Tasks;
using Agents;

namespace Game
{
    public class BotFeature : BaseFeature, IBot
    {
        [Inject] public BotRecord Record { get; set; }
        [Inject] public BattleUnitsRecord BattleUnitsRecord { get; set; }
        [Inject] public IPlayerAccount PlayerAccount { get; set; }

        public UniTask OnBeforeBattleTurnStart()
        {
            // Find all bot-controlled units and order their turns
            foreach (var unit in BattleUnitsRecord.BattleUnits)
            {
                if (IsBotUnit(unit))
                {
                    OrderBotTurn(unit).Forget();
                }
            }

            return UniTask.CompletedTask;
        }

        public UniTask OnBattleTurnStarted()
        {
            // Bot turn has started - currently no action needed
            return UniTask.CompletedTask;
        }

        public UniTask OnBattleTurnEnded()
        {
            // Bot turn has ended - currently no action needed
            return UniTask.CompletedTask;
        }

        public UniTask OrderBotTurn(BattleUnitData botUnit)
        {
            // TODO: Implement bot decision making logic
            // For now, bot units do nothing (empty turn)
            Notebook.NoteData($"Bot {botUnit.BattleUnitId} at {botUnit.Coordinate} is ordering turn (currently empty)");
            return UniTask.CompletedTask;
        }

        private bool IsBotUnit(BattleUnitData unit)
        {
            return PlayerAccount.IsBotPlayer(unit.PlayerId);
        }
    }
}