using Core;

namespace Game
{
    public class BattleUnits : BaseVisualFeature<BattleUnitsVisual>, IBattleUnits
    {
        [Inject] public BattleUnitsRecord Record { get; set; }
    }
}