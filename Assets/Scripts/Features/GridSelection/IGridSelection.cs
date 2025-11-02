using Core;

namespace Game
{
    public interface IGridSelection : IFeature
    {
        void Start();
        void Halt();
    }
}