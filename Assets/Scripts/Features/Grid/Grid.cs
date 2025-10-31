using Core;

namespace Game
{
    public class Grid : BaseVisualFeature<GridVisual>, IGrid
    {
        [Inject] public GridRecord Record { get; set; }
    }
}