using Core;

namespace Game
{
    public class BattleAssetsFeature : BaseVisualFeature<BattleAssetsVisual>, IBattleAssets
    {
        [Inject] public BattleAssetsRecord Record { get; set; }
        
        public int GoldAmount => Record.Gold;
    }
}