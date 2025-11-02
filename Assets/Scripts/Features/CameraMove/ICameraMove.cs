using Core;

namespace Game
{
    public interface ICameraMove : IFeature
    {
        void InitializeBounds();
        void Start();
        void Halt();
    }
}