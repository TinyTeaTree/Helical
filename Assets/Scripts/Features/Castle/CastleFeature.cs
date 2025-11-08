using Core;

namespace Game
{
    public class CastleFeature : BaseVisualFeature<CastleVisual>, ICastle
    {
        [Inject] public CastleRecord Record { get; set; }
    }
}