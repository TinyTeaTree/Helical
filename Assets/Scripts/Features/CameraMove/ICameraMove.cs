using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface ICameraMove : IFeature
    {
        UniTask SetupVisual();
        void InitializeBounds();
        void Start();
        void Halt();
    }
}