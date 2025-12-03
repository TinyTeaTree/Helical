using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public interface ITurn : IFeature
    {
        UniTask Start();

        void SelectedMyUnit();
        void OrderTurn(Vector2Int unitCoordinate, Vector2Int targetCoordinate, AbilityMode ability);
    }
}