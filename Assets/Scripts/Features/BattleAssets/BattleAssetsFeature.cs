using Agents;
using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class BattleAssetsFeature : BaseVisualFeature<BattleAssetsVisual>, IBattleAssets, IBattleLaunchAgent
    {
        [Inject] public BattleAssetsRecord Record { get; set; }

        public int GoldAmount => Record.Gold;

        public async UniTask BattleLaunch()
        {
            // Initialize with starting gold
            Record.Gold = 200;

            // Show the visual
            await CreateVisual();
            _visual.Show();
        }

        public bool CanAfford(int amount)
        {
            return Record.Gold >= amount;
        }

        public bool TrySpendGold(int amount)
        {
            if (!CanAfford(amount))
            {
                return false;
            }

            Record.Gold -= amount;
            _visual.UpdateGoldDisplay();
            return true;
        }

        public void AddGold(int amount)
        {
            Record.Gold += amount;
            _visual.UpdateGoldDisplay();
        }
    }
}