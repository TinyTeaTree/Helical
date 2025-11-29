using Core;

namespace Game
{
    public class TurnFeature : BaseVisualFeature<TurnVisual>, ITurn
    {
        [Inject] public TurnRecord Record { get; set; }
        
        private TurnBarVisual _turnBarVisual;
        
        public void ProvideTurnBar(TurnBarVisual turnBarVisual)
        {
            _turnBarVisual = turnBarVisual;
        }
    }
}