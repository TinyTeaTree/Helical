using Core;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface IGrid : IFeature
    {
        UniTask LoadGrid(string gridId);
    }
}