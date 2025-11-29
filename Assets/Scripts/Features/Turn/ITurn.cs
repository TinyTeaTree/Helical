using Core;

namespace Game
{
    public interface ITurn : IFeature
    {
        void ProvideTurnBar(TurnBarVisual turnBarVisual);
    }
}