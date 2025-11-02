using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface IGridSelection : IFeature
    {
        UniTask SetupVisual();
        void Start();
        void Halt();
        GridSelectionRecord Record { get; }
    }
}