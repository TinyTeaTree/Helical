using Core;

namespace Game
{
    public class Castle : BaseVisualFeature<CastleVisual>, ICastle
    {
        [Inject] public CastleRecord Record { get; set; }
    }
}