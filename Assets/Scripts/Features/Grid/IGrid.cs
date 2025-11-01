using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface IGrid : IFeature
    {
        GridVisual Visual { get; }
        UniTask LoadGrid(string gridId);
        Vector3 GetWorldPosition(Vector2Int coordinate);
    }
}